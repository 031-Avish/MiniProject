using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Services;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleFlightController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleFlightController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost("AddSchedule")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {
                ScheduleReturnDTO returnDTO = await _scheduleService.AddSchedule(scheduleDTO);
                return Ok(returnDTO);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (FlightRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpPut("UpdateSchedule")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> UpdateSchedule(ScheduleReturnDTO scheduleReturnDTO)
        {
            try
            {
                ScheduleReturnDTO updatedSchedule = await _scheduleService.UpdateSchedule(scheduleReturnDTO);
                return Ok(updatedSchedule);
            }
            catch (RouteInfoRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (FlightRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpDelete("DeleteSchedule/{scheduleId}")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> DeleteSchedule(int scheduleId)
        {
            try
            {
                ScheduleReturnDTO deletedSchedule = await _scheduleService.DeleteSchedule(scheduleId);
                return Ok(deletedSchedule);
            }
            catch (BookingRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetAllSchedules")]
        [ProducesResponseType(typeof(List<ScheduleDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleDetailDTO>>> GetAllSchedules()
        {
            try
            {
                List<ScheduleDetailDTO> schedules = await _scheduleService.GetAllSchedules();
                return Ok(schedules);
            }
            catch (ScheduleRepositoryException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("GetFlightDetailsOnDate")]
        [ProducesResponseType(typeof(List<ScheduleDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduleDetailDTO>>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO)
        {
            try
            {
                List<ScheduleDetailDTO> schedules = await _scheduleService.GetFlightDetailsOnDate(searchDTO);
                return Ok(schedules);
            }
            catch (NotPresentException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("GetConnectingFlights")]
        [ProducesResponseType(typeof(List<List<ScheduleDetailDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<List<ScheduleDetailDTO>>>> GetConnectingFlights(ScheduleSearchDTO searchDTO)
        {
            try
            {
                List<List<ScheduleDetailDTO>> connectingFlights = await _scheduleService.GetConnectingFlights(searchDTO);
                return Ok(connectingFlights);
            }
            catch (NotPresentException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ScheduleRepositoryException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (ScheduleServiceException ex)
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
