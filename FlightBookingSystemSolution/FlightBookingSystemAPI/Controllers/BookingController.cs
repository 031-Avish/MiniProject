using Microsoft.AspNetCore.Mvc;
using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage user bookings.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IUserBookingService _userBookingService;
        private readonly ILogger<BookingController> _logger;

        /// <summary>
        /// Constructor for BookingController.
        /// </summary>
        /// <param name="userBookingService">Service to manage user bookings.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public BookingController(IUserBookingService userBookingService, ILogger<BookingController> logger)
        {
            _userBookingService = userBookingService;
            _logger = logger;
        }


        /// <summary>
        /// Books a flight for the user.
        /// </summary>
        /// <param name="bookingDTO">Booking details.</param>
        /// <returns>Returns the booking confirmation details.</returns>
        #region BookFlight
        
        [Authorize(Roles = "User")]
        [HttpPost("BookFlightByUser")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> BookFlight(List<BookingDTO> bookingDTO)
        {
            try
            {
                _logger.LogInformation("User booking a flight.");
                var bookingReturnDTO = await _userBookingService.BookFlight(bookingDTO);
                _logger.LogInformation("Flight booked successfully.");
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while booking flight.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while booking flight.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Cancels a booking for the user.
        /// </summary>
        /// <param name="bookingId">ID of the booking to cancel.</param>
        /// <returns>Returns the cancellation details.</returns>
        #region CancelBooking
        
        [Authorize(Roles = "User")]
        [HttpDelete("CancelBookingByUser/{bookingId}")]
        [ProducesResponseType(typeof(BookingCancelReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingCancelReturnDTO>> CancelBooking(int bookingId)
        {
            try
            {
                _logger.LogInformation("User cancelling booking with ID {BookingId}.", bookingId);
                var bookingReturnDTO = await _userBookingService.CancelBooking(bookingId);
                _logger.LogInformation("Booking cancelled successfully.");
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling booking with ID {BookingId}.", bookingId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while cancelling booking with ID {BookingId}.", bookingId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Retrieves booking details for the user.
        /// </summary>
        /// <param name="bookingId">ID of the booking to retrieve.</param>
        /// <returns>Returns the booking details.</returns>
        #region GetBookingDetails
        
        [Authorize(Roles = "User")]
        [HttpGet("GetBookingDetailsByUser/{bookingId}")]
        [ProducesResponseType(typeof(BookingReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingReturnDTO>> GetBookingDetails(int bookingId)
        {
            try
            {
                _logger.LogInformation("User retrieving booking details for booking ID {BookingId}.", bookingId);
                var bookingReturnDTO = await _userBookingService.GetBookingDetails(bookingId);
                _logger.LogInformation("Booking details retrieved successfully.");
                return Ok(bookingReturnDTO);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking details for booking ID {BookingId}.", bookingId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving booking details for booking ID {BookingId}.", bookingId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Retrieves all bookings for the admin.
        /// </summary>
        /// <returns>Returns a list of all bookings.</returns>
        
        #region GetAllBookingsByAdmin
        
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllBookingsByAdmin")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllBookingsByAdmin()
        {
            try
            {
                _logger.LogInformation("Admin retrieving all bookings.");
                var bookings = await _userBookingService.GetAllBookingsByAdmin();
                _logger.LogInformation("All bookings retrieved successfully.");
                return Ok(bookings);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all bookings.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all bookings.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Retrieves all bookings for a user.
        /// </summary>
        /// <param name="userId">ID of the user whose bookings to retrieve.</param>
        /// <returns>Returns a list of the user's bookings.</returns>
        
        #region GetAllBookingsByUser
        
        [Authorize(Roles = "User")]
        [HttpGet("GetAllBookingsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetAllBookingsByUser(int userId)
        {
            try
            {
                _logger.LogInformation("User retrieving all bookings for user ID {UserId}.", userId);
                var bookings = await _userBookingService.GetAllBookingsByUser(userId);
                _logger.LogInformation("All bookings for user ID {UserId} retrieved successfully.", userId);
                return Ok(bookings);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all bookings for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all bookings for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Retrieves upcoming flights for a user.
        /// </summary>
        /// <param name="userId">ID of the user whose upcoming flights to retrieve.</param>
        /// <returns>Returns a list of the user's upcoming flights.</returns>
        #region GetUpcomingFlightsByUser

        [Authorize(Roles = "User")]
        [HttpGet("GetUpcomingFlightsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<List<BookingReturnDTO>>> GetUpcomingFlightsByUser(int userId)
        {
            try
            {
                _logger.LogInformation("User retrieving upcoming flights for user ID {UserId}.", userId);
                var flights = await _userBookingService.GetUpcomingFlightsByUser(userId);
                _logger.LogInformation("Upcoming flights for user ID {UserId} retrieved successfully.", userId);
                return Ok(flights);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving upcoming flights for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving upcoming flights for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


        /// <summary>
        /// Retrieves old flights for a user.
        /// </summary>
        /// <param name="userId">ID of the user whose old flights to retrieve.</param>
        /// <returns>Returns a list of the user's old flights.</returns>
        #region GetOldFlightsByUser
        
        [Authorize(Roles = "User")]
        [HttpGet("GetOldFlightsByUser/{userId}")]
        [ProducesResponseType(typeof(List<BookingReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BookingReturnDTO>>> GetOldFlightsByUser(int userId)
        {
            try
            {
                _logger.LogInformation("User retrieving old flights for user ID {UserId}.", userId);
                var flights = await _userBookingService.GetOldFlightsByUser(userId);
                _logger.LogInformation("Old flights for user ID {UserId} retrieved successfully.", userId);
                return Ok(flights);
            }
            catch (BookingServiceException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving old flights for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving old flights for user ID {UserId}.", userId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion
    }
}
