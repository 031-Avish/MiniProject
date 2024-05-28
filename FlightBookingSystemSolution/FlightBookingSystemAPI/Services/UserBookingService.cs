using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Services
{
    public class UserBookingService : IUserBookingService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, BookingDetail> _bookingDetailRepository;
        private readonly IRepository<int, Passenger> _passengerRepository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly FlightBookingContext _dbContext;

        public UserBookingService(
            IRepository<int, Booking> bookingRepository,
            IRepository<int, BookingDetail> bookingDetailRepository,
            IRepository<int, Passenger> passengerRepository,
            IRepository<int, Schedule> scheduleRepository,
            IRepository<int, User> userRepository,
            FlightBookingContext dbContext)
        {
            _bookingRepository = bookingRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _passengerRepository = passengerRepository;
            _scheduleRepository = scheduleRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
        }

        public async Task<BookingReturnDTO> BookFlight(BookingDTO bookingDTO)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            //using var transaction = await _bookingRepository.BeginTransactionAsync();
            try
            {
                // Get the schedule details
                var schedule = await _scheduleRepository.GetByKey(bookingDTO.ScheduleId);

                // Check seat availability and reduce available seats immediately
                int totalPassengers = bookingDTO.Passengers.Count(p => p.Age > 2);
                if (!CheckSeatAvailability(schedule, totalPassengers))
                {
                    throw new BookingServiceException("Not enough available seats for the requested booking.");
                }
                await ReduceAvailableSeats(schedule, totalPassengers);

                // Calculate total price
                float totalPrice = CalculateTotalPrice(schedule, bookingDTO.Passengers);

                // Apply first-time user discount
                totalPrice = await ApplyFirstTimeUserDiscount(bookingDTO.UserId, totalPrice);

                // Create a new booking
                Booking booking = new Booking
                {
                    BookingStatus = "Processing",
                    TotalPrice = totalPrice,
                    UserId = bookingDTO.UserId,
                    ScheduleId = bookingDTO.ScheduleId
                };

                var addedBooking = await _bookingRepository.Add(booking);

                // Add passenger details to booking
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
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapBookingToBookingReturnDTO(addedBooking);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BookingServiceException("Error occurred while booking the flight."+ex.Message, ex);
            }
        }

        // Helper methods

        private bool CheckSeatAvailability(Schedule schedule, int totalPassengers)
        {
            return schedule.AvailableSeat >= totalPassengers;
        }

        private float CalculateTotalPrice(Schedule schedule, List<PassengerDTO> passengers)
        {
            int paidSeats = passengers.Count(p => p.Age > 2);
            float totalPrice = schedule.Price * paidSeats;
            return totalPrice;
        }

        private async Task<float> ApplyFirstTimeUserDiscount(int userId, float totalPrice)
        {
            var userBookings = await _bookingRepository.GetAll();
            bool isFirstBooking = !userBookings.Any(b => b.UserId == userId);

            if (isFirstBooking)
            {
                totalPrice *= 0.95f; // Apply 5% discount
            }

            return totalPrice;
        }

        private async Task ReduceAvailableSeats(Schedule schedule, int seatsToReduce)
        {
            schedule.AvailableSeat -= seatsToReduce;
            await _scheduleRepository.Update(schedule);
        }

        private BookingReturnDTO MapBookingToBookingReturnDTO(Booking booking)
        {
            var bookingDetails = _bookingDetailRepository.GetAll().Result
                .Where(bd => bd.BookingId == booking.BookingId)
                .Select(bd => bd.PassengerDetail)
                .ToList();

            return new BookingReturnDTO
            {
                BookingId = booking.BookingId,
                BookingStatus = booking.BookingStatus,
                BookingDate = booking.BookingDate,
                PaymentStatus = booking.PaymentStatus,
                TotalPrice = booking.TotalPrice,
                UserId = booking.UserId,
                ScheduleId = booking.ScheduleId,
                FlightDetails = new ScheduleBookingDTO
                {
                    DepartureTime = booking.FlightDetails.DepartureTime,
                    ReachingTime = booking.FlightDetails.ReachingTime,
                    RouteId = booking.FlightDetails.RouteId,
                    FlightId = booking.FlightDetails.FlightId
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

        public Task<BookingReturnDTO> CancelBooking(int bookingId)
        {
            throw new NotImplementedException();
        }

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
                                b.BookingStatus == "Success")
                    .ToList();

                // Map the filtered bookings to DTOs
                var oldFlights = filteredBookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return oldFlights;
            }
            catch (Exception ex)
            {
                throw new BookingServiceException("Error occurred while getting old flights for the user." + ex.Message, ex);
            }
        }

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
                                b.BookingStatus == "Success")
                    .ToList();

                // Map the filtered bookings to DTOs
                var upcomingFlights = filteredBookings.Select(b => MapBookingToBookingReturnDTO(b)).ToList();

                return upcomingFlights;
            }
            catch (Exception ex)
            {
                throw new BookingServiceException("Error occurred while getting upcoming flights for the user."+ ex.Message, ex);
            }
        }



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
                throw new BookingServiceException("Error occurred while getting booking details.", ex);
            }
        }

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
                throw new BookingServiceException("Error occurred while getting all bookings.", ex);
            }
        }

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
                throw new BookingServiceException($"Error occurred while getting bookings for user with ID {userId}.", ex);
            }
        }
    }
}
