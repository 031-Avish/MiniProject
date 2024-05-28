using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.FlightDTO
{
    public class FlightReturnDTO
    {
        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Total seats is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1")]
        public int TotalSeats { get; set; }
    }
}
