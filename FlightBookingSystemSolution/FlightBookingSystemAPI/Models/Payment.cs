using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public float Amount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime PaymentDate { get; set; }= DateTime.Now;
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking BookingInfo { get; set; }    

    }
}
