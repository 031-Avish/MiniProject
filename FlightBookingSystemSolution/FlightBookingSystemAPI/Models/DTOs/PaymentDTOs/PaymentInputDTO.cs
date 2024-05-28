using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.PaymentDTOs
{
    public class PaymentInputDTO
    {
        [Required]
        public float Amount { get; set; }
        [Required]
        public int BookingId { get; set; }
    }
}
