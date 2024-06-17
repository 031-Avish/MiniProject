using Microsoft.AspNetCore.Mvc;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using Microsoft.AspNetCore.Authorization;

namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage payment-related tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        /// <summary>
        /// Constructor for PaymentController.
        /// </summary>
        /// <param name="paymentService">Service to manage payments.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        #region GetAllPayments

        /// <summary>
        /// Retrieves all payments (Admin only).
        /// </summary>
        /// <returns>Returns a list of all payments.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllPaymentsByAdmin")]
        [ProducesResponseType(typeof(List<PaymentReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentReturnDTO>>> GetAllPayments()
        {
            try
            {
                _logger.LogInformation("Retrieving all payments.");
                var payments = await _paymentService.GetAllPayments();
                _logger.LogInformation("All payments retrieved successfully.");
                return Ok(payments);
            }
            catch (PaymentServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all payments.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region GetPaymentById

        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        /// <param name="paymentId">ID of the payment to retrieve.</param>
        /// <returns>Returns the payment details.</returns>
        [HttpGet("GetPaymentById/{paymentId}")]
        [ProducesResponseType(typeof(PaymentReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentReturnDTO>> GetPaymentById(int paymentId)
        {
            try
            {
                _logger.LogInformation("Retrieving payment with ID {PaymentId}.", paymentId);
                var payment = await _paymentService.GetPaymentById(paymentId);
                _logger.LogInformation("Payment with ID {PaymentId} retrieved successfully.", paymentId);
                return Ok(payment);
            }
            catch (PaymentServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving payment with ID {PaymentId}.", paymentId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region GetPaymentsByBookingId

        /// <summary>
        /// Retrieves all payments for a specific booking.
        /// </summary>
        /// <param name="bookingId">ID of the booking to retrieve payments for.</param>
        /// <returns>Returns a list of payments for the specified booking.</returns>
        [HttpGet("GetPaymentsByBookingId/{bookingId}")]
        [ProducesResponseType(typeof(List<PaymentReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentReturnDTO>>> GetPaymentsByBookingId(int bookingId)
        {
            try
            {
                _logger.LogInformation("Retrieving payments for booking ID {BookingId}.", bookingId);
                var payments = await _paymentService.GetPaymentsByBookingId(bookingId);
                _logger.LogInformation("Payments for booking ID {BookingId} retrieved successfully.", bookingId);
                return Ok(payments);
            }
            catch (PaymentServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving payments for booking ID {BookingId}.", bookingId);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region ProcessPayment

        /// <summary>
        /// Processes a payment (User only).
        /// </summary>
        /// <param name="paymentDTO">Payment details.</param>
        /// <returns>Returns the processed payment details.</returns>
        [Authorize(Roles = "User")]
        [HttpPost("ProcessPaymentByUser")]
        [ProducesResponseType(typeof(PaymentReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentReturnDTO>> ProcessPayment(PaymentInputDTO paymentDTO)
        {
            try
            {
                _logger.LogInformation("Processing payment.");
                var payment = await _paymentService.ProcessPayment(paymentDTO);
                _logger.LogInformation("Payment processed successfully.");
                return Ok(payment);
            }
            catch (PaymentServiceException ex)
            {
                _logger.LogError(ex, "Error processing payment.");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (PaymentRepositoryException ex)
            {
                _logger.LogError(ex, "Repository error processing payment.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (BookingRepositoryException ex)
            {
                _logger.LogError(ex, "Booking repository error processing payment.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        #endregion
    }
}
