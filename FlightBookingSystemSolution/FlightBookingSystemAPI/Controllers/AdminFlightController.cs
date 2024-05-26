using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFlightController : ControllerBase
    {
        private readonly IAdminFlightService _adminFlightService;

        public AdminFlightController(IAdminFlightService adminFlightService)
        {
            _adminFlightService = adminFlightService;
        }

        [HttpPost("AddFlight")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> AddFlight(FlightDTO flightDTO)
        {
            try
            {
                FlightReturnDTO returnDTO = await _adminFlightService.AddFlight(flightDTO);
                return Ok(returnDTO);
            }
            catch (FlightRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpPut("UpdateFlight")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> UpdateFlight(FlightReturnDTO flightReturnDTO)
        {
            try
            {
                FlightReturnDTO updatedFlight = await _adminFlightService.UpdateFlight(flightReturnDTO);
                return Ok(updatedFlight);
            }
            catch (FlightRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpDelete("DeleteFlight/{flightId}")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> DeleteFlight(int flightId)
        {
            try
            {
                FlightReturnDTO deletedFlight = await _adminFlightService.DeleteFlight(flightId);
                return Ok(deletedFlight);
            }
            catch (FlightRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetFlight/{flightId}")]
        [ProducesResponseType(typeof(FlightReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightReturnDTO>> GetFlight(int flightId)
        {
            try
            {
                FlightReturnDTO flight = await _adminFlightService.GetFlight(flightId);
                return Ok(flight);
            }
            catch (FlightRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminFlightServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetAllFlights")]
        [ProducesResponseType(typeof(List<FlightReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<FlightReturnDTO>>> GetAllFlights()
        {
            try
            {
                List<FlightReturnDTO> flights = await _adminFlightService.GetAllFlight();
                return Ok(flights);
            }
            catch (FlightRepositoryException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (AdminFlightServiceException ex)
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
