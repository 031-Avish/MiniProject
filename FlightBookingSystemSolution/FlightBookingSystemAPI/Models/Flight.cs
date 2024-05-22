using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        public string Name {  get; set; }
        public int TotalSeats { get; set; }
    }
}
