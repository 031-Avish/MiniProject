using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleReturnDTO
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Reaching time is required")]
        public DateTime ReachingTime { get; set; }

        [Required(ErrorMessage = "Available seat count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Available seat count must be non-negative")]
        public int AvailableSeat { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }
    }
}
