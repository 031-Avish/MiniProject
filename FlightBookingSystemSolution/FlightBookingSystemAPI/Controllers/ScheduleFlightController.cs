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
using Microsoft.AspNetCore.Authorization;
using FlightBookingSystemAPI.Models;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleFlightController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        public ScheduleFlightController(IScheduleService scheduleService,IRepository<int,Schedule> sr)
        {
            _scheduleService = scheduleService;
            _scheduleRepository= sr;
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost("AddSchedule")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {
                var schedule = _scheduleRepository.Add(new Schedule());
                //ScheduleReturnDTO returnDTO = await _scheduleService.AddSchedule(scheduleDTO);
                return Ok(new ScheduleReturnDTO());
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
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateSchedule")]
        [ProducesResponseType(typeof(ScheduleReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScheduleReturnDTO>> UpdateSchedule(ScheduleUpdateDTO scheduleUpdateDTO)
        {
            try
            {
                ScheduleReturnDTO updatedSchedule = await _scheduleService.UpdateSchedule(scheduleUpdateDTO);
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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


        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "User")]
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
