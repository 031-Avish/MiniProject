using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing user bookings.
    /// </summary>
    public class UserBookingService : IUserBookingService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, BookingDetail> _bookingDetailRepository;
        private readonly IRepository<int, Passenger> _passengerRepository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly FlightBookingContext _dbContext;
        private readonly ILogger<UserBookingService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBookingService"/> class.
        /// </summary>
        /// <param name="bookingRepository">The booking repository.</param>
        /// <param name="bookingDetailRepository">The booking detail repository.</param>
        /// <param name="passengerRepository">The passenger repository.</param>
        /// <param name="scheduleRepository">The schedule repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public UserBookingService(
            IRepository<int, Booking> bookingRepository,
            IRepository<int, BookingDetail> bookingDetailRepository,
            IRepository<int, Passenger> passengerRepository,
            IRepository<int, Schedule> scheduleRepository,
            IRepository<int, User> userRepository,
            FlightBookingContext dbContext,
            ILogger<UserBookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _passengerRepository = passengerRepository;
            _scheduleRepository = scheduleRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Books a flight based on the provided booking details.
        /// </summary>
        /// <param name="bookingDTO">The booking details.</param>
        /// <returns>The booked flight details.</returns>
        #region BookFlight
        public async Task<List<BookingReturnDTO>> BookFlight(List<BookingDTO> bookingDTO)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Attempting to book a flight for user with ID {UserId}.", bookingDTO[0].UserId);

                List<BookingReturnDTO> bookingReturnDTOs = new List<BookingReturnDTO>();
                // Get the schedule details
                for (int i = 0; i < bookingDTO.Count; i++)
                {
                    var schedule = await _scheduleRepository.GetByKey(bookingDTO[i].ScheduleId);
                    if (schedule.DepartureTime < DateTime.Now)
                    {
                        throw new BookingServiceException("Cannot book for old schedule.");
                    }

                    _logger.LogInformation("Checking seat availability for schedule with ID {ScheduleId}.", bookingDTO[i].ScheduleId);

                    // Check seat availability and reduce available seats immediately
                    int totalPassengers = bookingDTO[i].Passengers.Count(p => p.Age > 2);
                    if (!CheckSeatAvailability(schedule, totalPassengers))
                    {
                        throw new BookingServiceException("Not enough available seats for the requested booking.");
                    }

                    _logger.LogInformation("Reducing available seats for schedule with ID {ScheduleId} by {TotalPassengers}.", bookingDTO[i]    .ScheduleId, totalPassengers);

                    // Reduce the available seats (Lock Seat)
                    await ReduceAvailableSeats(schedule, totalPassengers);

                    _logger.LogInformation("Calculating total price for booking.");

                    // Calculate total price
                    float totalPrice = CalculateTotalPrice(schedule, bookingDTO[i].Passengers);

                    _logger.LogInformation("Applying first-time user discount for user with ID {UserId}.", bookingDTO[i].UserId);

                    // Apply first-time user discount
                    totalPrice = await ApplyFirstTimeUserDiscount(bookingDTO[i].UserId, totalPrice);

                    _logger.LogInformation("Creating a new booking for user with ID {UserId}.", bookingDTO[i].UserId);

                    // Create a new booking
                    Booking booking = new Booking
                    {
                        BookingStatus = "Processing",
                        TotalPrice = totalPrice,
                        UserId = bookingDTO[i].UserId,
                        ScheduleId = bookingDTO[i].ScheduleId,
                        PassengerCount = bookingDTO[i].Passengers.Count
                    };

                    // Add the booking with status as Processing
                    var addedBooking = await _bookingRepository.Add(booking);

                    _logger.LogInformation("Adding passenger details and booking details for booking with ID {BookingId}.", addedBooking.BookingId);

                    // Add passenger details and booking details for each passenger
                    await AddBookingDetailsAndPassenger(bookingDTO[i], addedBooking);

                    await _dbContext.SaveChangesAsync();

                    bookingReturnDTOs.Add(MapBookingToBookingReturnDTO(addedBooking));
                    _logger.LogInformation("Booking successfully completed for user with ID {UserId}.", bookingDTO[i].UserId);

                    // Commit transaction if everything goes correct
                    
                }
                await transaction.CommitAsync();
                return bookingReturnDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking the flight for user with ID {UserId}.", bookingDTO[0].UserId);

                // Roll back and throw error if anything went wrong
                await transaction.RollbackAsync();
                throw new BookingServiceException("Error occurred while booking the flight.", ex);
            }
        }

        private async Task AddBookingDetailsAndPassenger(BookingDTO bookingDTO, Booking addedBooking)
        {
            foreach (var passengerDTO in bookingDTO.Passengers)
            {
                Passenger passenger = new Passenger
                {
                    Name = passengerDTO.Name,
                    Age = passengerDTO.Age,
                    Gender = passengerDTO.Gender
                };
                var addedPassenger = await _passengerRepository.Add(passenger);

                BookingDetail bookingDetail = new BookingDetail
                {
                    BookingId = addedBooking.BookingId,
                    PassengerId = addedPassenger.PassengerId
                };

                await _bookingDetailRepository.Add(bookingDetail);
            }
        }
        #endregion

        /// <summary>
        /// Cancels a booking by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>The canceled booking details.</returns>
        #region CancelBooking
        public async Task<BookingCancelReturnDTO> CancelBooking(int bookingId)
        {
           using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                {
                    _logger.LogInformation("Attempting to cancel the booking with ID {BookingId}.", bookingId);

                    // Get the booking entity from the database
                    var booking = await _bookingRepository.GetByKey(bookingId);

                    // Check if the booking status is "Completed" and the payment status is "Success"
                    if (booking.BookingStatus != "Completed" || booking.PaymentStatus != "Success")
                    {
                        throw new BookingServiceException("Cannot cancel the booking. Invalid booking state.");
                    }

                    _logger.LogInformation("Calculating refund amount for the canceled booking with ID {BookingId}.", bookingId);

                    // Calculate refund amount based on total price
                    var refundAmount = CalculateRefundAmount(booking.TotalPrice, booking.FlightDetails.DepartureTime);

                    _logger.LogInformation("Releasing the booked seats and increasing available seats for the canceled booking with ID {BookingId}.", bookingId);

                    // Release the booked seats and increase available seats
                    var schedule = await _scheduleRepository.GetByKey(booking.ScheduleId);
                    schedule.AvailableSeat += booking.PassengerCount;
                    await _scheduleRepository.Update(schedule);

                    _logger.LogInformation("Updating the booking status to 'Canceled' for the canceled booking with ID {BookingId}.", bookingId);

                    // Update the booking status to "Canceled"
                    booking.BookingStatus = "Canceled";

                    // Update the booking entity in the database
                    await _bookingRepository.Update(booking);

                    _logger.LogInformation("Committing the cancellation of the booking with ID {BookingId}.", bookingId);

                    // Commit the transaction
                 
                    await transaction.CommitAsync();

                    _logger.LogInformation("Cancellation of booking with ID {BookingId} successful.", bookingId);

                    // Return the canceled booking details with refund amount
                    return new BookingCancelReturnDTO
                    {
                        BookingId = booking.BookingId,
                        RefundAmount = refundAmount,
                        Message = "Cancellation successful. Refund will be credited in the next 3 days."
                    };
                }
            }
            catch (BookingRepositoryException)
            {
                await transaction.RollbackAsync();

                throw;
            }
            catch (ScheduleRepositoryException)
            {
                await transaction.RollbackAsync();

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling the booking with ID {BookingId}.", bookingId);
                throw new BookingServiceException("Error occurred while canceling the booking.", ex);
            }
        }
        #endregion
        #region CalculateRefundAmount
        private float CalculateRefundAmount(float totalPrice, DateTime departureTime)
        {
            // Get the difference in hours between the current time and the departure time
            TimeSpan timeDifference = departureTime - DateTime.Now;
            double hoursDifference = timeDifference.TotalHours;

            if (hoursDifference >= 48)
            {
                // Refund 75% of the total price
                return 0.75f * totalPrice;
            }
            else if (hoursDifference >= 12 && hoursDifference < 48)
            {
                // Refund 50% of the total price
                return 0.5f * totalPrice;
            }
            else
            {
                // No refund if canceled within 12 hours
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// Retrieves old flights for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of old flights.</returns>
        #region GetOldFlightsByUser
        public async Task<List<BookingReturnDTO>> GetOldFlightsByUser(int userId)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;

                var oldBookings = await _bookingRepository.GetAll();

                // Filter bookings for the given user ID with departure time before the current date and time and status as Success
                var filteredBookings = oldBookings
                    .Where(b => b.UserId == userId &&
                                b.FlightDetails.DepartureTime < currentDateTime &&
                                b.BookingStatus == "Completed")
                    .ToList();

                // Map the filtered bookings to DTOs
                var oldFlights = filteredBookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return oldFlights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting old flights for the user with ID {UserId}.", userId);
                throw new BookingServiceException("Error occurred while getting old flights for the user." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves upcoming flights for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of upcoming flights.</returns>
        #region GetUpcomingFlightsByUser
        public async Task<List<BookingReturnDTO>> GetUpcomingFlightsByUser(int userId)
        {
            try
            {
                DateTime currentDateTime = DateTime.Now;

                var upcomingBookings = await _bookingRepository.GetAll();

                // Filter bookings for the given user ID with departure time after the current date and time and status as Success
                var filteredBookings = upcomingBookings
                    .Where(b => b.UserId == userId &&
                                b.FlightDetails.DepartureTime > currentDateTime &&
                                b.BookingStatus == "Completed")
                    .ToList();

                // Map the filtered bookings to DTOs
                var upcomingFlights = filteredBookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return upcomingFlights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting upcoming flights for the user with ID {UserId}.", userId);
                throw new BookingServiceException("Error occurred while getting upcoming flights for the user." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves booking details by its ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>The booking details.</returns>
        #region GetBookingDetails
        public async Task<BookingReturnDTO> GetBookingDetails(int bookingId)
        {
            try
            {
                // Retrieve the booking entity from the database
                var booking = await _bookingRepository.GetByKey(bookingId);

                // Check if the booking exists
                if (booking == null)
                {
                    throw new BookingServiceException($"Booking with ID {bookingId} not found.");
                }

                // Map the booking entity to a BookingReturnDTO object
                var bookingDetails = MapBookingToBookingReturnDTO(booking);

                return bookingDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting booking details for the booking with ID {BookingId}.", bookingId);
                throw new BookingServiceException("Error occurred while getting booking details.", ex);
            }
        }
        #endregion


        /// <summary>
        /// Retrieves all bookings for admin.
        /// </summary>
        /// <returns>A list of all bookings.</returns>
        #region GetAllBookingsByAdmin
        public async Task<List<BookingReturnDTO>> GetAllBookingsByAdmin()
        {
            try
            {
                // Retrieve all bookings from the database
                var bookings = await _bookingRepository.GetAll();

                // Map the booking entities to BookingReturnDTO objects
                var bookingDetails = bookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return bookingDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all bookings for admin.");
                throw new BookingServiceException("Error occurred while getting all bookings.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all bookings for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of all bookings for the user.</returns>
        #region GetAllBookingsByUser
        public async Task<List<BookingReturnDTO>> GetAllBookingsByUser(int userId)
        {
            try
            {
                // Retrieve all bookings from the database
                var bookings = await _bookingRepository.GetAll();

                // Filter bookings for the specified user
                var userBookings = bookings.Where(b => b.UserId == userId);

                // Map the filtered booking entities to BookingReturnDTO objects
                var bookingDetails = userBookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return bookingDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting bookings for user with ID {UserId}.", userId);
                throw new BookingServiceException($"Error occurred while getting bookings for user with ID {userId}.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Checks seat availability for the given schedule and total passengers.
        /// </summary>
        /// <param name="schedule">The schedule entity.</param>
        /// <param name="totalPassengers">The total number of passengers.</param>
        /// <returns>True if enough seats are available, false otherwise.</returns>
        #region CheckSeatAvailability
        private bool CheckSeatAvailability(Schedule schedule, int totalPassengers)
        {
            return schedule.AvailableSeat >= totalPassengers;
        }
        #endregion

        /// <summary>
        /// Calculates the total price for the booking.
        /// </summary>
        /// <param name="schedule">The schedule entity.</param>
        /// <param name="passengers">The list of passengers.</param>
        /// <returns>The total price.</returns>
        #region CalculateTotalPrice
        private float CalculateTotalPrice(Schedule schedule, List<PassengerDTO> passengers)
        {
            int paidSeats = passengers.Count(p => p.Age > 2);
            float totalPrice = schedule.Price * paidSeats;
            return totalPrice;
        }
        #endregion

        /// <summary>
        /// Applies a first-time user discount to the total price.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="totalPrice">The total price before discount.</param>
        /// <returns>The total price after discount.</returns>
        #region ApplyFirstTimeUserDiscount
        private async Task<float> ApplyFirstTimeUserDiscount(int userId, float totalPrice)
        {
            IEnumerable<Booking> userBookings = null;
            try
            {
                userBookings = await _bookingRepository.GetAll();
            }
            // if no booking the 5% discount 
            catch (BookingRepositoryException ex) when (ex.Message.Contains("No bookings present"))
            {
                _logger.LogInformation("User with ID {UserId} gets a 5% discount on the first booking.");
                totalPrice *= 0.95f; // Apply 5% discount
                return totalPrice;
            }
            catch (BookingRepositoryException)
            {
                throw;
            }// check if it is users first booking 
            bool isFirstBooking = !userBookings.Any(b => b.UserId == userId);
            if (isFirstBooking)
            {
                _logger.LogInformation("User with ID {UserId} gets a 5% discount on the first booking.");
                totalPrice *= 0.95f; // Apply 5% discount
                return totalPrice;
            }
            return totalPrice;
        }
        #endregion

        /// <summary>
        /// Reduces the available seats for the given schedule.
        /// </summary>
        /// <param name="schedule">The schedule entity.</param>
        /// <param name="seatsToReduce">The number of seats to reduce.</param>
        #region ReduceAvailableSeats
        private async Task ReduceAvailableSeats(Schedule schedule, int seatsToReduce)
        {
            schedule.AvailableSeat -= seatsToReduce;
            await _scheduleRepository.Update(schedule);
        }
        #endregion


        /// <summary>
        /// Maps a booking entity to a BookingReturnDTO object.
        /// </summary>
        /// <param name="booking">The booking entity.</param>
        /// <returns>The booking return DTO.</returns>
        #region DTOs
        private BookingReturnDTO MapBookingToBookingReturnDTO(Booking booking1)
        {
            var bookingDetails = _bookingDetailRepository.GetAll().Result
                .Where(bd => bd.BookingId == booking1.BookingId)
                .Select(bd => bd.PassengerDetail)
                .ToList();
            var booking = _bookingRepository.GetAll().Result
                .Where(b=>b.BookingId == booking1.BookingId).ToList();
            ;
            return new BookingReturnDTO
            {
                BookingId = booking[0].BookingId,
                BookingStatus = booking[0].BookingStatus,
                BookingDate = booking[0].BookingDate,
                PaymentStatus = booking[0].PaymentStatus,
                TotalPrice = booking[0].TotalPrice,
                UserId = booking[0].UserId,
                ScheduleId = booking[0].ScheduleId,

                FlightDetails = new ScheduleDetailDTO
                {
                    DepartureTime = booking[0].FlightDetails.DepartureTime,
                    ReachingTime = booking[0].FlightDetails.ReachingTime,
                    RouteInfo = MapToRouteReturnDTO(booking[0].FlightDetails.RouteInfo),
                    FlightInfo = MapBookingToFlightDTO(booking[0].FlightDetails.FlightInfo),
                },
                Passengers = bookingDetails.Select(pd => new PassengerReturnDTO
                {
                    PassengerId = pd.PassengerId,
                    Name = pd.Name,
                    Age = pd.Age,
                    Gender = pd.Gender
                }).ToList()
            };
        }

        private FlightReturnDTO MapBookingToFlightDTO(Flight flightInfo)
        {
            return new FlightReturnDTO
            {
                FlightId = flightInfo.FlightId,
                Name = flightInfo.Name,
                TotalSeats = flightInfo.TotalSeats,
            };
        }

        private RouteInfoReturnDTO MapToRouteReturnDTO(RouteInfo routeInfo)
        {
            return new RouteInfoReturnDTO
            {
                RouteId = routeInfo.RouteId,
                StartCity = routeInfo.StartCity,
                EndCity = routeInfo.EndCity,
                Distance = routeInfo.Distance,
            };
        }
        #endregion
    }
}
