using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Repositories;
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
        public UserLoginRegisterController(IUserService userService)
        {
            _userService = userService;
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
    }
}
