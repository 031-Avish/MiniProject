using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleBookingDTO
    {
        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Reaching time is required")]
        public DateTime ReachingTime { get; set; }

        [Required(ErrorMessage = "Route ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Route ID must be greater than 0")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Flight ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Flight ID must be greater than 0")]
        public int FlightId { get; set; }
    }
}
