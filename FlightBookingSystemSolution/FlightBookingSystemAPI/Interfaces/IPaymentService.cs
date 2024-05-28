using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentReturnDTO> ProcessPayment(PaymentInputDTO paymentDTO);
        Task<List<PaymentReturnDTO>> GetAllPayments();
        Task<PaymentReturnDTO> GetPaymentById(int paymentId);
        Task<List<PaymentReturnDTO>> GetPaymentsByBookingId(int bookingId);
    }
}
