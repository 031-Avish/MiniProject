using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.PaymentDTOs
{
    public class PaymentInputDTO
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, float.MaxValue, ErrorMessage = "Amount must be a positive number")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }
    }
}
