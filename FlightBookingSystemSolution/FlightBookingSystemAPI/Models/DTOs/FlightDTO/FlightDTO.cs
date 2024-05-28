using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.FlightDTO
{
    public class FlightDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int TotalSeats { get; set; }
    }
}
