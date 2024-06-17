using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage flight-related administrative tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFlightController : ControllerBase
    {
        private readonly IAdminFlightService _adminFlightService;
        private readonly ILogger<AdminFlightController> _logger;

        /// <summary>
        /// Constructor for AdminFlightController.
        /// </summary>
        /// <param name="adminFlightService">Service to manage flights.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public AdminFlightController(IAdminFlightService adminFlightService, 
                                    ILogger<AdminFlightController> logger)
        {
            _adminFlightService = adminFlightService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new flight.
        /// </summary>
        /// <param name="flightDTO">Flight details.</param>
        /// <returns>Returns the added flight details.</returns>
        #region AddFlight

        [Authorize(Roles = "Admin")]
        [HttpPost("AddFlight")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> AddFlight(FlightDTO flightDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new flight.");
                FlightReturnDTO returnDTO = await _adminFlightService.AddFlight(flightDTO);
                _logger.LogInformation("Flight added successfully.");
                return Ok(returnDTO);
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error adding flight.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Service error adding flight.");
                return BadRequest( new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding flight.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing flight.
        /// </summary>
        /// <param name="flightReturnDTO">Updated flight details.</param>
        /// <returns>Returns the updated flight details.</returns>
        #region UpdateFlight

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateFlight")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> UpdateFlight(FlightReturnDTO flightReturnDTO)
        {
            try
            {
                _logger.LogInformation("Updating flight with ID {FlightId}.", flightReturnDTO.FlightId);
                FlightReturnDTO updatedFlight = await _adminFlightService.UpdateFlight(flightReturnDTO);
                _logger.LogInformation("Flight with ID {FlightId} updated successfully.", flightReturnDTO.FlightId);
                return Ok(updatedFlight);
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error updating flight with ID {FlightId}.", flightReturnDTO.FlightId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Service error updating flight with ID {FlightId}.", flightReturnDTO.FlightId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating flight with ID {FlightId}.", flightReturnDTO.FlightId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Deletes a flight.
        /// </summary>
        /// <param name="flightId">ID of the flight to delete.</param>
        /// <returns>Returns details of the deleted flight.</returns>
        #region DeleteFlight

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteFlight/{flightId}")]
        [ProducesResponseType(typeof(FlightDeleteReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightDeleteReturnDTO>> DeleteFlight(int flightId)
        {
            try
            {
                _logger.LogInformation("Deleting flight with ID {FlightId}.", flightId);
                FlightDeleteReturnDTO deletedFlight = await _adminFlightService.DeleteFlight(flightId);
                _logger.LogInformation("Flight with ID {FlightId} deleted successfully.", flightId);
                return Ok(deletedFlight);
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error deleting flight with ID {FlightId}.", flightId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Service error deleting flight with ID {FlightId}.", flightId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting flight with ID {FlightId}.", flightId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Retrieves details of a flight by ID.
        /// </summary>
        /// <param name="flightId">ID of the flight to retrieve.</param>
        /// <returns>Returns the flight details.</returns>
        #region GetFlight
        
        [Authorize(Roles = "Admin")]
        [HttpGet("GetFlight/{flightId}")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> GetFlight(int flightId)
        {
            try
            {
                _logger.LogInformation("Retrieving flight with ID {FlightId}.", flightId);
                FlightReturnDTO flight = await _adminFlightService.GetFlight(flightId);
                _logger.LogInformation("Flight with ID {FlightId} retrieved successfully.", flightId);
                return Ok(flight);
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error retrieving flight with ID {FlightId}.", flightId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Service error retrieving flight with ID {FlightId}.", flightId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving flight with ID {FlightId}.", flightId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all flights.
        /// </summary>
        /// <returns>Returns a list of all flights.</returns>
        #region GetAllFlights

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllFlights")]
        [ProducesResponseType(typeof(List<FlightReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<FlightReturnDTO>>> GetAllFlights()
        {
            try
            {
                _logger.LogInformation("Retrieving all flights.");
                List<FlightReturnDTO> flights = await _adminFlightService.GetAllFlight();
                _logger.LogInformation("All flights retrieved successfully.");
                return Ok(flights);
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "Error retrieving all flights.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                _logger.LogError(ex, "Service error retrieving all flights.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving all flights.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion
    }
}
