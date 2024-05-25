using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginRegisterController : ControllerBase
    {
        public readonly IUserService _userService;
        public readonly IAdminService _adminService;
        public UserLoginRegisterController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }
        

        [HttpPost("Register")]
        [ProducesResponseType(typeof(UserRegisterReturnDTO) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserRegisterReturnDTO>> Register(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                UserRegisterReturnDTO returnDTO = await _userService.Register(userRegisterDTO);
                return Ok(returnDTO);
            }
            catch(UserRepositoryException e)
            {
                return BadRequest(new ErrorModel(400, e.Message));
            }
            catch(UnableToRegisterException e)
            {
                return BadRequest(new ErrorModel(400,e.Message));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(400, e.Message));
            }
        }
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                var result = await _userService.Login(userLoginDTO);
                return Ok(result);
            }
            catch(UserRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(UserDetailRepositoryException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(UnauthorizedUserException ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch(UnableToLoginException ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch(Exception ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

    }
}
