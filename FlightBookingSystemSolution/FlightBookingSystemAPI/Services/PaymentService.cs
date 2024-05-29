using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
using FlightBookingSystemAPI.Repositories;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Payment> _paymentRepository;
        private readonly IRepository<int,Schedule> _scheduleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="bookingRepository">The booking repository.</param>
        /// <param name="paymentRepository">The payment repository.</param>
        public PaymentService(IRepository<int, Booking> bookingRepository, IRepository<int, Payment> paymentRepository
            , IRepository<int, Schedule> scheduleRepo)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _scheduleRepository = scheduleRepo;
        }

        #region GetAllPayments
        /// <summary>
        /// Retrieves all payments.
        /// </summary>
        /// <returns>A list of all payment data transfer objects.</returns>
        public async Task<List<PaymentReturnDTO>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAll();
                return payments.Select(p => MapPaymentToReturnDTO(p)).ToList();
            }
            catch (PaymentRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException("Error occurred while retrieving all payments." + ex.Message, ex);
            }
        }
        #endregion

        #region GetPaymentById
        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment.</param>
        /// <returns>The payment data transfer object.</returns>
        public async Task<PaymentReturnDTO> GetPaymentById(int paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetByKey(paymentId);
                return MapPaymentToReturnDTO(payment);
            }
            catch (PaymentRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException($"Error occurred while retrieving payment with ID {paymentId}.", ex);
            }
        }
        #endregion

        #region GetPaymentsByBookingId
        /// <summary>
        /// Retrieves payments by booking ID.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>A list of payment data transfer objects for the specified booking.</returns>
        public async Task<List<PaymentReturnDTO>> GetPaymentsByBookingId(int bookingId)
        {
            try
            {
                var payments = await _paymentRepository.GetAll();
                var paymentsByBooking = payments.Where(p => p.BookingId == bookingId).ToList();
                return paymentsByBooking.Select(p => MapPaymentToReturnDTO(p)).ToList();
            }
            catch (PaymentRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException($"Error occurred while retrieving payments for booking with ID {bookingId}.", ex);
            }
        }
        #endregion

        #region ProcessPayment
        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="paymentDTO">The payment input data transfer object.</param>
        /// <returns>The processed payment data transfer object.</returns>
        public async Task<PaymentReturnDTO> ProcessPayment(PaymentInputDTO paymentDTO)
        {
            Booking booking = null;
            Payment payment = null;
            Schedule schedule = null;
            try
            {
                booking = await _bookingRepository.GetByKey(paymentDTO.BookingId);
                if (booking.BookingStatus != "Processing")
                {
                    throw new PaymentServiceException("Invalid booking ID as booking with this id is Failed or not present ");
                }

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

                if (isPaymentSuccessful)
                {
                    payment.PaymentStatus = "Success";
                    booking.BookingStatus = "Completed";
                    booking.PaymentStatus = "Success";
                }
                else
                {
                    schedule = await _scheduleRepository.GetByKey(booking.ScheduleId);
                    schedule.AvailableSeat += booking.PassengerCount;
                    payment.PaymentStatus = "Failed";
                    booking.BookingStatus = "Failed";
                    booking.PaymentStatus = "Failed";
                }
                await _scheduleRepository.Update(schedule);
                await _paymentRepository.Update(payment);
                await _bookingRepository.Update(booking);

                return MapPaymentToReturnDTO(payment);
            }
            catch (PaymentRepositoryException)
            { 
                await MakeChangesWhenException(booking, payment);
                throw;
            }
            catch (BookingRepositoryException)
            {
                await MakeChangesWhenException(booking, payment);
                throw;
            }
            catch (Exception ex)
            {
                await MakeChangesWhenException(booking, payment);
                throw new PaymentServiceException("Error occurred while processing the payment.", ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Maps a <see cref="Payment"/> to a <see cref="PaymentReturnDTO"/>.
        /// </summary>
        /// <param name="payment">The payment entity.</param>
        /// <returns>The payment data transfer object.</returns>
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
