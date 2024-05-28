using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
using FlightBookingSystemAPI.Repositories;

namespace FlightBookingSystemAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Payment> _paymentRepository;

        public PaymentService(IRepository<int, Booking> bookingRepository, IRepository<int, Payment> paymentRepository)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
        }
        public async Task<List<PaymentReturnDTO>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAll();
                return payments.Select(p => MapPaymentToReturnDTO(p)).ToList();
            }
            catch(PaymentRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PaymentServiceException("Error occurred while retrieving all payments."+ex.Message, ex);
            }
        }

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

        public async Task<PaymentReturnDTO> ProcessPayment(PaymentInputDTO paymentDTO)
        {
            Booking booking = null;
            Payment payment = null;
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
                    payment.PaymentStatus = "Failed";
                    booking.BookingStatus = "Failed";
                    booking.PaymentStatus = "Failed";
                }

                await _paymentRepository.Update(payment);
                await _bookingRepository.Update(booking);

                return MapPaymentToReturnDTO(payment);
            }
            catch (PaymentRepositoryException)
            {

                await MakechangesWhenException(booking, payment);
                throw;
            }
            catch (BookingRepositoryException)
            {
                await MakechangesWhenException(booking, payment);
                throw;
            }
            catch (Exception ex)
            {
                await MakechangesWhenException(booking, payment);
                throw new PaymentServiceException("Error occurred while processing the payment.", ex);
            }

        }
        private async Task MakechangesWhenException(Booking booking, Payment payment)
        {
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
        private bool SimulatePaymentProcessing(float amount)
        {
            
            return amount > 0; 
        }
    }
}
