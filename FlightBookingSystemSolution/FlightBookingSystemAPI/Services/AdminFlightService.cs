using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Repositories;

namespace FlightBookingSystemAPI.Services
{
    public class AdminFlightService : IAdminFlightService
    {
        private readonly IRepository<int, Flight> _repository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        public AdminFlightService(IRepository<int, Flight> repository, IRepository<int,Schedule> scheduleRepository)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
        }
        public async Task<FlightReturnDTO> AddFlight(FlightDTO flightDTO)
        {
            try
            {
                Flight flight = MapFlightWithFlightDTO(flightDTO);
                Flight AddedFlight = await _repository.Add(flight);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(AddedFlight);
                return flightReturnDTO;
            }
            catch (FlightRepositoryException fr)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminFlightServiceException("Cannot add Flight this Moment some unwanted error occured :", ex);
            }
        }
        private FlightReturnDTO MapFlightWithFlightReturnDTO(Flight addedFlight)
        {
            FlightReturnDTO flightReturnDTO = new FlightReturnDTO();
            flightReturnDTO.FlightId = addedFlight.FlightId;
            flightReturnDTO.Name = addedFlight.Name;
            flightReturnDTO.TotalSeats = addedFlight.TotalSeats;
            return flightReturnDTO;
        }
        private Flight MapFlightWithFlightDTO(FlightDTO flightDTO)
        {
            Flight flight = new Flight();
            flight.Name = flightDTO.Name;
            flight.TotalSeats = flightDTO.TotalSeats;
            return flight;
        }
        public async Task<FlightReturnDTO> DeleteFlight(int flightId)
        {
            try
            {
                // Check if the flight is scheduled
                //var scheduledFlights = await _scheduleRepository.GetAll();
                //foreach (var scheduledFlight in scheduledFlights)
                //{
                //    if (scheduledFlight.FlightId == flightId)
                //    {
                //        throw new AdminFlightServiceException("Cannot delete the flight because it is scheduled.");
                //    }
                //}

                // If not scheduled, then delete
                Flight flight = await _repository.DeleteByKey(flightId);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(flight);
                return flightReturnDTO;
            }
            catch (FlightRepositoryException)
            {
                throw;
            }
            catch (AdminFlightServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminFlightServiceException("Error occurred while deleting the flight: " + ex.Message, ex);
            }
        }

        public async Task<List<FlightReturnDTO>> GetAllFlight()
        {
            try
            {
                var flights = await _repository.GetAll();
                List<FlightReturnDTO> flightReturnDTOs = new List<FlightReturnDTO>();
                foreach (Flight flight in flights)
                {
                    flightReturnDTOs.Add(MapFlightWithFlightReturnDTO(flight));
                }
                return flightReturnDTOs;
            }
            catch (UserRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminFlightServiceException("Error Occured While Getting All the Flight" + ex.Message, ex);
            }
        }
        public async Task<FlightReturnDTO> GetFlight(int flightId)
        {
            try
            {
                Flight flight = await _repository.GetByKey(flightId);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(flight);
                return flightReturnDTO;
            }
            catch (UserRepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AdminFlightServiceException("Error Occured While Getting the Flight" + e.Message, e);
            }
        }

        public async Task<FlightReturnDTO> UpdateFlight(FlightReturnDTO FlightReturnDTO)
        {
            try
            {
                Flight flight = MapFlightReturnDTOWithFlight(FlightReturnDTO);
                Flight UpdatedFlight = await _repository.Update(flight);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(UpdatedFlight);
                return flightReturnDTO;
            }
            catch (UserRepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AdminFlightServiceException("Error Occured While Updating the Flight" + e.Message, e);
            }
        }

        private Flight MapFlightReturnDTOWithFlight(FlightReturnDTO flightReturnDTO)
        {
            Flight flight = new Flight();
            flight.FlightId = flightReturnDTO.FlightId;
            flight.Name = flightReturnDTO.Name;
            flight.TotalSeats = flightReturnDTO.TotalSeats;
            return flight;
        }
    }
}
