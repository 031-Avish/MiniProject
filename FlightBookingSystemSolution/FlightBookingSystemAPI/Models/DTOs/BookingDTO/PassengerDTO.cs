

using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.BookingDTO
{
    public class PassengerDTO
    {
        [Required(ErrorMessage = "Passenger name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Passenger age is required")]
        [Range(0, 150, ErrorMessage = "Passenger age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Passenger gender is required")]
        public string Gender { get; set; }
    }
}
