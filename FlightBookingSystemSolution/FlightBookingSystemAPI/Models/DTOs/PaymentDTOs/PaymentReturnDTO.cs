using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.PaymentDTOs
{
    public class PaymentReturnDTO
    {
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, float.MaxValue, ErrorMessage = "Amount must be a positive number")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        public string PaymentStatus { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }
    }
}
