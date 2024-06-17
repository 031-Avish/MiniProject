using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Payment> _paymentRepository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly ILogger<PaymentService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="bookingRepository">The booking repository.</param>
        /// <param name="paymentRepository">The payment repository.</param>
        /// <param name="logger">The logger.</param>
        #region Constructor 
        public PaymentService(IRepository<int, Booking> bookingRepository, IRepository<int, Payment> paymentRepository,
            IRepository<int, Schedule> scheduleRepo, ILogger<PaymentService> logger)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _scheduleRepository = scheduleRepo;
            _logger = logger;
        }
        #endregion

        #region GetAllPayments
        /// <summary>
        /// Retrieves all payments.
        /// </summary>
        /// <returns>A list of all payment data transfer objects.</returns>
        public async Task<List<PaymentReturnDTO>> GetAllPayments()
        {
            try
            {
                _logger.LogInformation("Retrieving all payments.");
                // get payment Details and map them 
                var payments = await _paymentRepository.GetAll();
                return payments.Select(p => MapPaymentToReturnDTO(p)).ToList();
            }
            catch (PaymentRepositoryException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all payments from the repository.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all payments.");
                throw new PaymentServiceException("Error occurred while retrieving all payments." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// get a payment by its ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment.</param>
        /// <returns>The payment data transfer object.</returns>
        #region GetPaymentById
        public async Task<PaymentReturnDTO> GetPaymentById(int paymentId)
        {
            try
            {
                _logger.LogInformation("Retrieving payment with ID {PaymentId}.", paymentId);
                // get payment by the given Id 
                var payment = await _paymentRepository.GetByKey(paymentId);
                return MapPaymentToReturnDTO(payment);
            }
            catch (PaymentRepositoryException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payment with ID {PaymentId} from the repository.", paymentId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving payment with ID {PaymentId}.", paymentId);
                throw new PaymentServiceException($"Error occurred while retrieving payment" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves payments by booking ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>A list of payment data transfer objects for the specified booking.</returns>
        #region GetPaymentsByBookingId
        public async Task<List<PaymentReturnDTO>> GetPaymentsByBookingId(int bookingId)
        {
            try
            {
                _logger.LogInformation("Retrieving payments for booking ID {BookingId}.", bookingId);
                // GetAllPayments 
                var payments = await _paymentRepository.GetAll();
                // Filter payment based on the given booking Id and return by mapping 
                var paymentsByBooking = payments.Where(p => p.BookingId == bookingId).ToList();
                if (paymentsByBooking.Count <= 0)
                {
                    _logger.LogWarning("No payments found for booking ID {BookingId}.", bookingId);
                    throw new PaymentServiceException("No Payment For given Booking Found");
                }
                return paymentsByBooking.Select(p => MapPaymentToReturnDTO(p)).ToList();
            }
            catch (PaymentRepositoryException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payments for booking ID {BookingId} from the repository.", bookingId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving payments for booking ID {BookingId}.", bookingId);
                throw new PaymentServiceException($"Error occurred while retrieving payments" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="paymentDTO">The payment input data transfer object.</param>
        /// <returns>The processed payment data transfer object.</returns>
        #region ProcessPayment
        public async Task<PaymentReturnDTO> ProcessPayment(PaymentInputDTO paymentDTO)
        {
            Booking booking = null;
            Payment payment = null;
            Schedule schedule = null;
            try
            {
                _logger.LogInformation("Processing payment for booking ID {BookingId}.", paymentDTO.BookingId);
                // payment can only be done when booking status is Processing 
                booking = await _bookingRepository.GetByKey(paymentDTO.BookingId);
                if (booking.BookingStatus != "Processing")
                {
                    _logger.LogWarning("Invalid booking ID {BookingId} as booking status is {BookingStatus}.", paymentDTO.BookingId, booking.BookingStatus);
                    throw new PaymentServiceException("Invalid booking ID as booking with this id is Failed or not present ");
                }
                // Add Payment 
                payment = new Payment
                {
                    Amount = paymentDTO.Amount,
                    BookingId = paymentDTO.BookingId,
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.Now
                };

                var addedPayment = await _paymentRepository.Add(payment);

                // Simulate payment processing
                bool isPaymentSuccessful = SimulatePaymentProcessing(paymentDTO.Amount);
                // if payment is successful make this changes 
                if (isPaymentSuccessful)
                {
                    payment.PaymentStatus = "Success";
                    booking.BookingStatus = "Completed";
                    booking.PaymentStatus = "Success";
                }
                else
                {
                    // if failed Release the seat and change status 
                    schedule = await _scheduleRepository.GetByKey(booking.ScheduleId);
                    schedule.AvailableSeat += booking.PassengerCount;
                    await _scheduleRepository.Update(schedule);
                    payment.PaymentStatus = "Failed";
                    booking.BookingStatus = "Failed";
                    booking.PaymentStatus = "Failed";
                }
                // update database 
                await _paymentRepository.Update(payment);
                await _bookingRepository.Update(booking);

                return MapPaymentToReturnDTO(payment);
            }
            catch (PaymentRepositoryException ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment for booking ID {BookingId}.", paymentDTO.BookingId);
                await MakeChangesWhenException(booking, payment);
                throw;
            }
            catch (BookingRepositoryException ex)
            {
                _logger.LogError(ex, "An error occurred in the booking repository while processing payment for booking ID {BookingId}.", paymentDTO.BookingId);
                await MakeChangesWhenException(booking, payment);
                throw;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing payment for booking ID {BookingId}.", paymentDTO.BookingId);
                await MakeChangesWhenException(booking, payment);
                throw new PaymentServiceException("Error occurred while processing the payment."+ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Maps a <see cref="Payment"/> to a <see cref="PaymentReturnDTO"/>.
        /// </summary>
        /// <param name="payment">The payment entity.</param>
        /// <returns>The payment data transfer object.</returns>
        #region Helper Methods
        private PaymentReturnDTO MapPaymentToReturnDTO(Payment payment)
        {
            return new PaymentReturnDTO
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate,
                BookingId = payment.BookingId
            };
        }

        /// <summary>
        /// Simulates payment processing.
        /// </summary>
        /// <param name="amount">The amount to process.</param>
        /// <returns><c>true</c> if the payment is successful; otherwise, <c>false</c>.</returns>
        private bool SimulatePaymentProcessing(float amount)
        {
            return amount > 0;
        }

        /// <summary>
        /// Makes changes when an exception occurs during payment processing.
        /// </summary>
        /// <param name="booking">The booking entity.</param>
        /// <param name="payment">The payment entity.</param>
        private async Task MakeChangesWhenException(Booking booking, Payment payment)
        {
            _logger.LogWarning("Reverting changes due to an error during payment processing.");
            if (booking == null) return;

            var schedule = await _scheduleRepository.GetByKey(booking.ScheduleId);
            schedule.AvailableSeat += booking.PassengerCount;
            await _scheduleRepository.Update(schedule);


            if (booking != null)
            {
                booking.BookingStatus = "Failed";
                booking.PaymentStatus = "Failed";
                await _bookingRepository.Update(booking);
            }
            if (payment != null)
            {
                payment.PaymentStatus = "Failed";
                await _paymentRepository.Update(payment);
            }
        }
        #endregion
    }
}
