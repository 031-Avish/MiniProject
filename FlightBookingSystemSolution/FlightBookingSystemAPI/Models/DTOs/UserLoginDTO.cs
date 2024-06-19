using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "User id is required")]
        //[Range(1,1000, ErrorMessage = "Invalid entry for User ID. User ID must be between 1 and 1000.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
