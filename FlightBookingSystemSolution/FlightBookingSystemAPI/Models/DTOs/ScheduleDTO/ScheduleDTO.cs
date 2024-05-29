using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleDTO
    {
        [Required(ErrorMessage = "Departure time is required")]
        [DateNotInPast]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Reaching time is required")]
        [DateNotInPast]
        public DateTime ReachingTime { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }
    }
}
