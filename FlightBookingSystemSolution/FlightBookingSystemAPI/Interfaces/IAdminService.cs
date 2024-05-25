using FlightBookingSystemAPI.Models;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IAdminService:IAdminFlightService
    {
        public Task<Flight> DeleteFlight(string id);
    }
}
