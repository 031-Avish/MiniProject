using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Models.DTOs;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IUserBookingService _userBookingService;

        public BookingController(IUserBookingService userBookingService)
        {
            _userBookingService = userBookingService;
        }

        [HttpPost("BookFlight")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingReturnDTO>> BookFlight(BookingDTO bookingDTO)
        {
            try
            {
                var bookingReturnDTO = await _userBookingService.BookFlight(bookingDTO);
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpDelete("CancelBooking/{bookingId}")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingReturnDTO>> CancelBooking(int bookingId)
        {
            try
            {
                var bookingReturnDTO = await _userBookingService.CancelBooking(bookingId);
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetBookingDetails/{bookingId}")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingReturnDTO>> GetBookingDetails(int bookingId)
        {
            try
            {
                var bookingReturnDTO = await _userBookingService.GetBookingDetails(bookingId);
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetAllBookingsByAdmin")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllBookingsByAdmin()
        {
            try
            {
                var bookings = await _userBookingService.GetAllBookingsByAdmin();
                return Ok(bookings);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetAllBookingsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllBookingsByUser(int userId)
        {
            try
            {
                var bookings = await _userBookingService.GetAllBookingsByUser(userId);
                return Ok(bookings);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetUpcomingFlightsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetUpcomingFlightsByUser(int userId)
        {
            try
            {
                var flights = await _userBookingService.GetUpcomingFlightsByUser(userId);
                return Ok(flights);
            }
            catch (BookingServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetOldFlightsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetOldFlightsByUser(int userId)
        {
            try
            {
                var flights = await _userBookingService.GetOldFlightsByUser(userId);
                return Ok(flights);
            }
            catch (BookingServiceException ex)
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
