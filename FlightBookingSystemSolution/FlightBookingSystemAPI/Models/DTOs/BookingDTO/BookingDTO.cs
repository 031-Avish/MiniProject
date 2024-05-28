
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.PassengerDTOs;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    public class BookingDTO
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Passengers information is required")]
        [MinLength(1, ErrorMessage = "At least one passenger must be provided")]
        public List<PassengerDTO> Passengers { get; set; }
    }
}
