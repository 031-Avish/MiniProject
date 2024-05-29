using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IAdminFlightService
    {
        public Task<FlightReturnDTO> AddFlight(FlightDTO flightDTO); 
        public Task<FlightReturnDTO> UpdateFlight(FlightReturnDTO FlightReturnDTO);
        public Task<FlightDeleteReturnDTO> DeleteFlight(int flightId);
        public Task<FlightReturnDTO> GetFlight(int flightId);
        public Task<List<FlightReturnDTO>> GetAllFlight();
    }
}
