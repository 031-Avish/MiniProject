using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.PassengerDTOs
{
    public class PassengerDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
    }
}
