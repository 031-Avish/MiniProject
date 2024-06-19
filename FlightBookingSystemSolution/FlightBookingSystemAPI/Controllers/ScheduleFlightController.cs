using Microsoft.AspNetCore.Mvc;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;

namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage flight schedules.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("MyCors")]
    public class ScheduleFlightController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<ScheduleFlightController> _logger;

        /// <summary>
        /// Constructor for ScheduleFlightController.
        /// </summary>
        /// <param name="scheduleService">Service to manage flight schedules.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public ScheduleFlightController(IScheduleService scheduleService, ILogger<ScheduleFlightController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        #region AddSchedule

        /// <summary>
        /// Adds a new flight schedule (Admin only).
        /// </summary>
        /// <param name="scheduleDTO">Schedule details.</param>
        /// <returns>Returns the added schedule details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("AddScheduleByAdmin")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new schedule.");
                ScheduleReturnDTO returnDTO = await _scheduleService.AddSchedule(scheduleDTO);
                _logger.LogInformation("Schedule added successfully.");
                return Ok(returnDTO);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "RouteInfoRepositoryException encountered while adding schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "FlightRepositoryException encountered while adding schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while adding schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while adding schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region UpdateSchedule

        /// <summary>
        /// Updates an existing flight schedule (Admin only).
        /// </summary>
        /// <param name="scheduleUpdateDTO">Updated schedule details.</param>
        /// <returns>Returns the updated schedule details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateScheduleByAdmin")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> UpdateSchedule(ScheduleUpdateDTO scheduleUpdateDTO)
        {
            try
            {
                _logger.LogInformation("Updating schedule.");
                ScheduleReturnDTO updatedSchedule = await _scheduleService.UpdateSchedule(scheduleUpdateDTO);
                _logger.LogInformation("Schedule updated successfully.");
                return Ok(updatedSchedule);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "RouteInfoRepositoryException encountered while updating schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (FlightRepositoryException ex)
            {
                _logger.LogError(ex, "FlightRepositoryException encountered while updating schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while updating schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while updating schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region DeleteSchedule

        /// <summary>
        /// Deletes a flight schedule by ID (Admin only).
        /// </summary>
        /// <param name="scheduleId">ID of the schedule to delete.</param>
        /// <returns>Returns the deleted schedule details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteScheduleByAdmin/{scheduleId}")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> DeleteSchedule(int scheduleId)
        {
            try
            {
                _logger.LogInformation("Deleting schedule with ID {ScheduleId}.", scheduleId);
                ScheduleReturnDTO deletedSchedule = await _scheduleService.DeleteSchedule(scheduleId);
                _logger.LogInformation("Schedule with ID {ScheduleId} deleted successfully.", scheduleId);
                return Ok(deletedSchedule);
            }
            catch (BookingRepositoryException ex)
            {
                _logger.LogError(ex, "BookingRepositoryException encountered while deleting schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while deleting schedule.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while deleting schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting schedule.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region GetAllSchedules

        /// <summary>
        /// Retrieves all flight schedules (Admin only).
        /// </summary>
        /// <returns>Returns a list of all schedules.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllSchedulesByAdmin")]
        [ProducesResponseType(typeof(List<ScheduleDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleDetailDTO>>> GetAllSchedules()
        {
            try
            {
                _logger.LogInformation("Retrieving all schedules.");
                List<ScheduleDetailDTO> schedules = await _scheduleService.GetAllSchedules();
                _logger.LogInformation("All schedules retrieved successfully.");
                return Ok(schedules);
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while retrieving all schedules.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while retrieving all schedules.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all schedules.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region GetFlightDetailsOnDate

        /// <summary>
        /// Retrieves flight details for a specific date (User only).
        /// </summary>
        /// <param name="searchDTO">Search criteria.</param>
        /// <returns>Returns a list of flight details.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("GetFlightDetailsOnDateByUser")]
        [ProducesResponseType(typeof(List<ScheduleDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleDetailDTO>>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO)
        {
            try
            {
                _logger.LogInformation("Retrieving flight details for date.");
                List<ScheduleDetailDTO> schedules = await _scheduleService.GetFlightDetailsOnDate(searchDTO);
                _logger.LogInformation("Flight details for date retrieved successfully.");
                return Ok(schedules);
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "NotPresentException encountered while retrieving flight details.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while retrieving flight details.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while retrieving flight details.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving flight details.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region GetConnectingFlights

        /// <summary>
        /// Retrieves connecting flights based on search criteria (User only).
        /// </summary>
        /// <param name="searchDTO">Search criteria.</param>
        /// <returns>Returns a list of connecting flights.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("GetConnectingFlightsByUser")]
        [ProducesResponseType(typeof(List<List<ScheduleDetailDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<List<ScheduleDetailDTO>>>> GetConnectingFlights(ScheduleSearchDTO searchDTO)
        {
            try
            {
                _logger.LogInformation("Retrieving connecting flights.");
                List<List<ScheduleDetailDTO>> connectingFlights = await _scheduleService.GetConnectingFlights(searchDTO);
                _logger.LogInformation("Connecting flights retrieved successfully.");
                return Ok(connectingFlights);
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "NotPresentException encountered while retrieving connecting flights.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                _logger.LogError(ex, "ScheduleRepositoryException encountered while retrieving connecting flights.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                _logger.LogError(ex, "ScheduleServiceException encountered while retrieving connecting flights.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving connecting flights.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion
    }
}
