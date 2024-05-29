using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleDetailDTO
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        [DateNotInPast]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Reaching time is required")]
        [DateNotInPast]
        public DateTime ReachingTime { get; set; }

        [Required(ErrorMessage = "Available seat count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Available seat count must be greater than or equal to 0")]
        public int AvailableSeat { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Route information is required")]
        public RouteInfoReturnDTO RouteInfo { get; set; }

        [Required(ErrorMessage = "Flight information is required")]
        public FlightReturnDTO FlightInfo { get; set; }
    }
}
