namespace FlightBookingSystemAPI.Models.DTOs
{
    public class UserRegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string password { get; set; }
    }
}
