using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Controllers
{
    /// <summary>
    /// Controller to manage user registration and login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("MyCors")]
    public class UserLoginRegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserLoginRegisterController> _logger;

        /// <summary>
        /// Constructor for UserLoginRegisterController.
        /// </summary>
        /// <param name="userService">Service to manage user operations.</param>
        /// <param name="logger">Logger to log actions and errors.</param>
        public UserLoginRegisterController(IUserService userService, ILogger<UserLoginRegisterController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        #region Register

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegisterDTO">User registration details.</param>
        /// <returns>Returns the registered user details.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(UserRegisterReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserRegisterReturnDTO>> Register(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                _logger.LogInformation("Registering a new user.");
                UserRegisterReturnDTO returnDTO = await _userService.Register(userRegisterDTO);
                _logger.LogInformation("User registered successfully.");
                return Ok(returnDTO);
            }
            catch (UserServiceException e)
            {
                _logger.LogError(e, "UserServiceException encountered during user registration.");
                return BadRequest(new ErrorModel(400, e.Message));
            }
            catch (UnableToRegisterException e)
            {
                _logger.LogError(e, "UnableToRegisterException encountered during user registration.");
                return BadRequest(new ErrorModel(400, e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred during user registration.");
                return BadRequest(new ErrorModel(400, e.Message));
            }
        }

        #endregion

        #region Login

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="userLoginDTO">User login details.</param>
        /// <returns>Returns the login result.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                _logger.LogInformation("Logging in user.");
                var result = await _userService.Login(userLoginDTO);
                _logger.LogInformation("User logged in successfully.");
                return Ok(result);
            }
            catch (UserServiceException ex)
            {
                _logger.LogError(ex, "UserServiceException encountered during user login.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UserDetailRepositoryException ex)
            {
                _logger.LogError(ex, "UserDetailRepositoryException encountered during user login.");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UnauthorizedUserException ex)
            {
                _logger.LogError(ex, "UnauthorizedUserException encountered during user login.");
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (UnableToLoginException ex)
            {
                _logger.LogError(ex, "UnableToLoginException encountered during user login.");
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during user login.");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        #endregion
    }
}
