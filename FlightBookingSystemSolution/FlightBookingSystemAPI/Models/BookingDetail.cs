using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingSystemAPI.Models
{
    public class BookingDetail
    {
        [Key]
        public int BookingDetailId { get; set; }
        public int PassengerId { get; set; }
        [ForeignKey("PassengerId")]
        public Passenger PassengerDetail { get; set; }
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Bookings { get; set; }
    }
}
