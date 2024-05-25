
using FlightBookingSystemAPI.Models;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken( User user);
    }
}
