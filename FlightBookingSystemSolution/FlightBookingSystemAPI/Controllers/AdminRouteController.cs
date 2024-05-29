using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage route-related administrative tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminRouteController : ControllerBase
    {
        private readonly IAdminRouteInfoService _adminRouteInfoService;
        private readonly ILogger<AdminRouteController> _logger;

        /// <summary>
        /// Constructor for AdminRouteController.
        /// </summary>
        /// <param name="adminRouteInfoService">Service to manage route information.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public AdminRouteController(IAdminRouteInfoService adminRouteInfoService, ILogger<AdminRouteController> logger)
        {
            _adminRouteInfoService = adminRouteInfoService;
            _logger = logger;
        }

        #region AddRouteInfo
        /// <summary>
        /// Adds a new route information.
        /// </summary>
        /// <param name="routeInfoDTO">Route information details.</param>
        /// <returns>Returns the added route information details.</returns>
        [HttpPost("AddRouteInfo")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> AddRouteInfo(RouteInfoDTO routeInfoDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new route information.");
                RouteInfoReturnDTO returnDTO = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
                _logger.LogInformation("Route information added successfully.");
                return Ok(returnDTO);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error adding route information.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                _logger.LogError(ex, "Service error adding route information.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding route information.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region UpdateRouteInfo
        /// <summary>
        /// Updates existing route information.
        /// </summary>
        /// <param name="routeInfoReturnDTO">Updated route information details.</param>
        /// <returns>Returns the updated route information details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRouteInfo")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> UpdateRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO)
        {
            try
            {
                _logger.LogInformation("Updating route information with ID {RouteId}.", routeInfoReturnDTO.RouteId);
                RouteInfoReturnDTO updatedRouteInfo = await _adminRouteInfoService.UpdateRouteInfo(routeInfoReturnDTO);
                _logger.LogInformation("Route information with ID {RouteId} updated successfully.", routeInfoReturnDTO.RouteId);
                return Ok(updatedRouteInfo);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error updating route information with ID {RouteId}.", routeInfoReturnDTO.RouteId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                _logger.LogError(ex, "Service error updating route information with ID {RouteId}.", routeInfoReturnDTO.RouteId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating route information with ID {RouteId}.", routeInfoReturnDTO.RouteId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region DeleteRouteInfo
        /// <summary>
        /// Deletes route information.
        /// </summary>
        /// <param name="routeInfoId">ID of the route information to delete.</param>
        /// <returns>Returns details of the deleted route information.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRouteInfo/{routeInfoId}")]
        [ProducesResponseType(typeof(RouteInfoDeleteReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoDeleteReturnDTO>> DeleteRouteInfo(int routeInfoId)
        {
            try
            {
                _logger.LogInformation("Deleting route information with ID {RouteId}.", routeInfoId);
                RouteInfoDeleteReturnDTO deletedRouteInfo = await _adminRouteInfoService.DeleteRouteInfo(routeInfoId);
                _logger.LogInformation("Route information with ID {RouteId} deleted successfully.", routeInfoId);
                return Ok(deletedRouteInfo);
            }
            catch (UnableToDeleteRouteInfoException ex)
            {
                _logger.LogError(ex, "Error deleting route information with ID {RouteId}.", routeInfoId);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Route information with ID {RouteId} not found.", routeInfoId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                _logger.LogError(ex, "Service error deleting route information with ID {RouteId}.", routeInfoId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting route information with ID {RouteId}.", routeInfoId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region GetRouteInfo
        /// <summary>
        /// Retrieves details of route information by ID.
        /// </summary>
        /// <param name="routeInfoId">ID of the route information to retrieve.</param>
        /// <returns>Returns the route information details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetRouteInfo/{routeInfoId}")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> GetRouteInfo(int routeInfoId)
        {
            try
            {
                _logger.LogInformation("Retrieving route information with ID {RouteId}.", routeInfoId);
                RouteInfoReturnDTO routeInfo = await _adminRouteInfoService.GetRouteInfo(routeInfoId);
                _logger.LogInformation("Route information with ID {RouteId} retrieved successfully.", routeInfoId);
                return Ok(routeInfo);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Route information with ID {RouteId} not found.", routeInfoId);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                _logger.LogError(ex, "Service error retrieving route information with ID {RouteId}.", routeInfoId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving route information with ID {RouteId}.", routeInfoId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region GetAllRouteInfos
        /// <summary>
        /// Retrieves all route informations.
        /// </summary>
        /// <returns>Returns a list of all route informations.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRouteInfos")]
        [ProducesResponseType(typeof(List<RouteInfoReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RouteInfoReturnDTO>>> GetAllRouteInfos()
        {
            try
            {
                _logger.LogInformation("Retrieving all route informations.");
                List<RouteInfoReturnDTO> routeInfos = await _adminRouteInfoService.GetAllRouteInfos();
                _logger.LogInformation("All route informations retrieved successfully.");
                return Ok(routeInfos);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error retrieving all route informations.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                _logger.LogError(ex, "Service error retrieving all route informations.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving all route informations.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion
    }
}
