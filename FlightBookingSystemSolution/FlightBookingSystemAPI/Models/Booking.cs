using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public string BookingStatus { get; set; }
        public DateTime BookingDate { get; set;} = DateTime.Now;
        public string PaymentStatus { get; set; } = "Pending";
        public float TotalPrice { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]

        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule FlightDetails { get; set; }

    }
}
