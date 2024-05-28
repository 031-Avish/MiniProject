using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs
{
    public class UserRegisterReturnDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }
    }
}
