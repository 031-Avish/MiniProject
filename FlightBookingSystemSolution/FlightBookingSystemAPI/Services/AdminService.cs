using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;

namespace FlightBookingSystemAPI.Services
{
    public class AdminService : AdminFlightService, IAdminService
    {
        public Task<Flight> DeleteFlight(string id)
        {
            throw new NotImplementedException();
        }
    }
}
