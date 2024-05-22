using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public float Amount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking BookingInfo { get; set; }    

    }
}
