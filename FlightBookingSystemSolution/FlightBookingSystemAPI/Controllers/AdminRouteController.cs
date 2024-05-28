using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminRouteController : ControllerBase
    {
        private readonly IAdminRouteInfoService _adminRouteInfoService;

        public AdminRouteController(IAdminRouteInfoService adminRouteInfoService)
        {
            _adminRouteInfoService = adminRouteInfoService;
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost("AddRouteInfo")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> AddRouteInfo(RouteInfoDTO routeInfoDTO)
        {
            try
            {
                RouteInfoReturnDTO returnDTO = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
                return Ok(returnDTO);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRouteInfo")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> UpdateRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO)
        {
            try
            {
                RouteInfoReturnDTO updatedRouteInfo = await _adminRouteInfoService.UpdateRouteInfo(routeInfoReturnDTO);
                return Ok(updatedRouteInfo);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRouteInfo/{routeInfoId}")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> DeleteRouteInfo(int routeInfoId)
        {
            try
            {
                RouteInfoReturnDTO deletedRouteInfo = await _adminRouteInfoService.DeleteRouteInfo(routeInfoId);
                return Ok(deletedRouteInfo);
            }
            catch (UnableToDeleteRouteInfoException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (RouteInfoRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetRouteInfo/{routeInfoId}")]
        [ProducesResponseType(typeof(RouteInfoReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteInfoReturnDTO>> GetRouteInfo(int routeInfoId)
        {
            try
            {
                RouteInfoReturnDTO routeInfo = await _adminRouteInfoService.GetRouteInfo(routeInfoId);
                return Ok(routeInfo);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllRouteInfos")]
        [ProducesResponseType(typeof(List<RouteInfoReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RouteInfoReturnDTO>>> GetAllRouteInfos()
        {
            try
            {
                List<RouteInfoReturnDTO> routeInfos = await _adminRouteInfoService.GetAllRouteInfos();
                return Ok(routeInfos);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (AdminRouteInfoServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
