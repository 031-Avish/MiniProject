using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs
{
    public class LoginReturnDTO
    {
        [Required(ErrorMessage = "User id is required")]
        [Range(1,1000, ErrorMessage = "Invalid entry for user ID. User ID must be between 1 and 1000.") ]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}
