using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing flights for admins.
    /// </summary>
    public class AdminFlightService : IAdminFlightService
    {
        private readonly IRepository<int, Flight> _repository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly ILogger<AdminFlightService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminFlightService"/> class.
        /// </summary>
        /// <param name="repository">The flight repository.</param>
        /// <param name="scheduleRepository">The schedule repository.</param>
        /// <param name="logger">The logger instance.</param>
        public AdminFlightService(IRepository<int, Flight> repository, IRepository<int, Schedule> scheduleRepository, ILogger<AdminFlightService> logger)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new flight.
        /// </summary>
        /// <param name="flightDTO">The flight data transfer object.</param>
        /// <returns>The added flight as a data transfer object.</returns>
        #region AddFlight
        public async Task<FlightReturnDTO> AddFlight(FlightDTO flightDTO)
        {
            try
            {
                // Add FLight and map it 
                _logger.LogInformation("Adding a new flight.");
                Flight flight = MapFlightWithFlightDTO(flightDTO);
                Flight addedFlight = await _repository.Add(flight);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(addedFlight);
                _logger.LogInformation("Flight added successfully.");
                return flightReturnDTO;
            }
            catch (FlightRepositoryException fr)
            {
                _logger.LogError(fr, "Error occurred while adding flight.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding flight.");
                throw new AdminFlightServiceException("Cannot add Flight at this moment. An unexpected error occurred:", ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a flight by its ID.
        /// </summary>
        /// <param name="flightId">The ID of the flight to delete.</param>
        /// <returns>The deleted flight as a data transfer object.</returns>
        #region DeleteFlight
        public async Task<FlightDeleteReturnDTO> DeleteFlight(int flightId)
        {
            try
            {
                IEnumerable<Schedule> schedules = null;
                try
                {
                    schedules = await _scheduleRepository.GetAll();
                } // if there is no schedule then we can delete the  flight 
                catch(ScheduleRepositoryException ex ) when (ex.Message.Contains("No schedules present"))
                {
                    _logger.LogInformation("Deleting flight with ID: {FlightId}", flightId);
                    Flight flight1 = await _repository.DeleteByKey(flightId);
                    FlightDeleteReturnDTO flightReturnDTO1 = MapFlightWithFlightDeleteReturnDTO(flight1);
                    _logger.LogInformation("Flight deleted successfully.");
                    return flightReturnDTO1;
                }
                // if there is schedule then check flight id is present or not 
                if(schedules.Any(schedule => schedule.FlightId == flightId))
                {// if present then check if there is any upcoming flight  
                    var updateFlight = await _repository.GetByKey(flightId);
                    //if upcoming schedule then cant delete 
                    if (schedules.Any(schedule => schedule.FlightId == flightId && schedule.ScheduleStatus == "Enable" && schedule.DepartureTime > DateTime.Now ))
                    {
                        throw new AdminFlightServiceException("cannot update flight !! Schedule has this flight Update that first");
                    }
                    // if old schedule then update status 
                    else
                    {
                        updateFlight.FlightStatus = "Disabled";
                        Flight flight2= await _repository.Update(updateFlight);
                        FlightDeleteReturnDTO flightReturnDTO2 = MapFlightWithFlightDeleteReturnDTO(flight2);
                        _logger.LogInformation("Flight Updated successfully.");
                        return flightReturnDTO2;
                    }
                }
                _logger.LogInformation("Deleting flight with ID: {FlightId}", flightId);
                Flight flight = await _repository.DeleteByKey(flightId);
                FlightDeleteReturnDTO flightReturnDTO = MapFlightWithFlightDeleteReturnDTO(flight);
                _logger.LogInformation("Flight deleted successfully.");
                return flightReturnDTO;
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting flight.");
                throw;
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting flight.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting flight.");
                throw new AdminFlightServiceException("Error occurred while deleting the flight: " + ex.Message, ex);
            }
        }

        private FlightDeleteReturnDTO MapFlightWithFlightDeleteReturnDTO(Flight addedFlight)
        {
            FlightDeleteReturnDTO flightReturnDTO = new FlightDeleteReturnDTO
            {
                FlightId = addedFlight.FlightId,
                Name = addedFlight.Name,
                TotalSeats = addedFlight.TotalSeats,
                FlightStatus= addedFlight.FlightStatus
            };
            return flightReturnDTO;
        }
        #endregion

        /// <summary>
        /// Gets all flights.
        /// </summary>
        /// <returns>A list of all flights as data transfer objects.</returns>
        #region GetAllFlights
        public async Task<List<FlightReturnDTO>> GetAllFlight()
        {
            try
            {
                // get all flight and Map to DTO
                _logger.LogInformation("Retrieving all flights.");
                var flights = await _repository.GetAll();
                List<FlightReturnDTO> flightReturnDTOs = new List<FlightReturnDTO>();
                foreach (Flight flight in flights)
                {
                    flightReturnDTOs.Add(MapFlightWithFlightReturnDTO(flight));
                }
                _logger.LogInformation("All flights retrieved successfully.");
                return flightReturnDTOs;
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all flights.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all flights.");
                throw new AdminFlightServiceException("Error occurred while getting all the flights: " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Gets a flight by its ID.
        /// </summary>
        /// <param name="flightId">The ID of the flight to retrieve.</param>
        /// <returns>The flight as a data transfer object.</returns>
        #region GetFlight
        public async Task<FlightReturnDTO> GetFlight(int flightId)
        {
            try
            {
                // get all flights with Flight Id 
                _logger.LogInformation("Retrieving flight with ID: {FlightId}", flightId);
                Flight flight = await _repository.GetByKey(flightId);
                FlightReturnDTO flightReturnDTO = MapFlightWithFlightReturnDTO(flight);
                _logger.LogInformation("Flight retrieved successfully.");
                return flightReturnDTO;
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving flight.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving flight.");
                throw new AdminFlightServiceException("Error occurred while getting the flight: " + ex.Message, ex);
            }
        }
        #endregion

        #region UpdateFlight
        /// <summary>
        /// Updates a flight.
        /// </summary>
        /// <param name="flightReturnDTO">The flight data transfer object containing the updated information.</param>
        /// <returns>The updated flight as a data transfer object.</returns>
        public async Task<FlightReturnDTO> UpdateFlight(FlightReturnDTO flightReturnDTO)
        {
            try
            {
                _logger.LogInformation("Updating flight with ID: {FlightId}", flightReturnDTO.FlightId);
                Flight flight = MapFlightReturnDTOWithFlight(flightReturnDTO);
                Flight updatedFlight = await _repository.Update(flight);
                FlightReturnDTO updatedFlightReturnDTO = MapFlightWithFlightReturnDTO(updatedFlight);
                _logger.LogInformation("Flight updated successfully.");
                return updatedFlightReturnDTO;
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating flight.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating flight.");
                throw new AdminFlightServiceException("Error occurred while updating the flight: " + ex.Message, ex);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Maps a <see cref="FlightDTO"/> to a <see cref="Flight"/>.
        /// </summary>
        /// <param name="flightDTO">The flight data transfer object.</param>
        /// <returns>A new <see cref="Flight"/> object.</returns>
        private Flight MapFlightWithFlightDTO(FlightDTO flightDTO)
        {
            Flight flight = new Flight
            {
                Name = flightDTO.Name,
                TotalSeats = flightDTO.TotalSeats
            };
            return flight;
        }

        /// <summary>
        /// Maps a <see cref="Flight"/> to a <see cref="FlightReturnDTO"/>.
        /// </summary>
        /// <param name="addedFlight">The added flight.</param>
        /// <returns>A new <see cref="FlightReturnDTO"/> object.</returns>
        private FlightReturnDTO MapFlightWithFlightReturnDTO(Flight addedFlight)
        {
            FlightReturnDTO flightReturnDTO = new FlightReturnDTO
            {
                FlightId = addedFlight.FlightId,
                Name = addedFlight.Name,
                TotalSeats = addedFlight.TotalSeats
            };
            return flightReturnDTO;
        }

        /// <summary>
        /// Maps a <see cref="FlightReturnDTO"/> to a <see cref="Flight"/>.
        /// </summary>
        /// <param name="flightReturnDTO">The flight return data transfer object.</param>
        /// <returns>A new <see cref="Flight"/> object.</returns>
        private Flight MapFlightReturnDTOWithFlight(FlightReturnDTO flightReturnDTO)
        {
            Flight flight = new Flight
            {
                FlightId = flightReturnDTO.FlightId,
                Name = flightReturnDTO.Name,
                TotalSeats = flightReturnDTO.TotalSeats
            };
            return flight;
        }
        #endregion
    }
}
