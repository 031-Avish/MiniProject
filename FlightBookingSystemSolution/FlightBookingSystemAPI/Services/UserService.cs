using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace FlightBookingSystemAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, UserDetail> _userDetailRepo;

        public UserService(IRepository<int,User> userRepo, IRepository<int, UserDetail> userDetailRepo)
        {
            _userRepo = userRepo;
            _userDetailRepo= userDetailRepo;
        }
        public Task<LoginReturnDTO> Login(UserLoginDTO userLoginDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<UserRegisterReturnDTO> Register(UserRegisterDTO userRegisterDTO)
        {
            User user = null ;
            UserDetail userDetail=null ;
            try
            {
                User user1 = GenerateUser(userRegisterDTO);
                UserDetail userDetail1 = MapUserRegisterDTOToUserDetail(userRegisterDTO);
                user= await _userRepo.Add(user1);
                userDetail1.UserId = user.UserId;
                userDetail = await _userDetailRepo.Add(userDetail1);
                UserRegisterReturnDTO userRegisterReturnDTO = MapUserToReturnDTO(user);
                return userRegisterReturnDTO;
            }
            catch(UserRepositoryException)
            {
                throw;
            }
            catch (Exception)
            {

            }
            if(user!=null)
            {
                await RevertUserInsert(user);
            }
            if(userDetail!=null)
            {
                await RevertUserDetailInsert(userDetail);
            }
            throw new UnableToRegisterException("Not Able to Register User at this moment");
        }

        private UserRegisterReturnDTO MapUserToReturnDTO(User user)
        {
            UserRegisterReturnDTO returnDTO = new UserRegisterReturnDTO();  
            returnDTO.UserId = user.UserId;
            returnDTO.Email = user.Email;
            returnDTO.Name = user.Name;
            returnDTO.Phone = user.Phone;
            return returnDTO;
        }

        private async Task RevertUserDetailInsert(UserDetail userDetail)
        {
            await _userDetailRepo.DeleteByKey(userDetail.UserId);
        }

        private async Task RevertUserInsert(User user)
        {
            await _userRepo.DeleteByKey(user.UserId);
        }

        private UserDetail MapUserRegisterDTOToUserDetail(UserRegisterDTO userRegisterDTO)
        {
            UserDetail userDetail = new UserDetail();
            HMACSHA512 hMACSHA512 = new HMACSHA512();
            userDetail.PasswordHashKey = hMACSHA512.Key;
            userDetail.Email = userRegisterDTO.Email;
            userDetail.Password = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.password));
            return userDetail;
        }

        private User GenerateUser(UserRegisterDTO userRegisterDTO)
        {
            User user = new User();
            user.Name = userRegisterDTO.Name;
            user.Email = userRegisterDTO.Email; 
            user.Phone = userRegisterDTO.Phone;
            return user;
        }
    }
}
