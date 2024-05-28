using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.PassengerDTO
{
    public class PassengerReturnDTO
    {
        [Required(ErrorMessage = "Passenger ID is required")]
        public int PassengerId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
    }
}
