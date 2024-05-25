using FlightBookingSystemAPI.Models.DTOs;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IUserService
    {
        public Task<LoginReturnDTO> Login(UserLoginDTO userLoginDTO);
        public Task<UserRegisterReturnDTO> Register(UserRegisterDTO userRegisterDTO);
    }
}
