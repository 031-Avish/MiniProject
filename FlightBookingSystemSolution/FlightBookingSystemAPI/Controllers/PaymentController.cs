using Microsoft.AspNetCore.Mvc;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
using FlightBookingSystemAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("GetAllPayments")]
        [ProducesResponseType(typeof(List<PaymentReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentReturnDTO>>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPayments();
                return Ok(payments);
            }
            catch (PaymentServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetPaymentById/{paymentId}")]
        [ProducesResponseType(typeof(PaymentReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentReturnDTO>> GetPaymentById(int paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentById(paymentId);
                return Ok(payment);
            }
            catch (PaymentServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpGet("GetPaymentsByBookingId/{bookingId}")]
        [ProducesResponseType(typeof(List<PaymentReturnDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PaymentReturnDTO>>> GetPaymentsByBookingId(int bookingId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByBookingId(bookingId);
                return Ok(payments);
            }
            catch (PaymentServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [HttpPost("ProcessPayment")]
        [ProducesResponseType(typeof(PaymentReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaymentReturnDTO>> ProcessPayment(PaymentInputDTO paymentDTO)
        {
            try
            {
                var payment = await _paymentService.ProcessPayment(paymentDTO);
                return Ok(payment);
            }
            catch (PaymentServiceException ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            catch (PaymentRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (BookingRepositoryException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
