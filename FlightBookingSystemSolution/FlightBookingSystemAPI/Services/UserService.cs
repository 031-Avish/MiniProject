using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace FlightBookingSystemAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, UserDetail> _userDetailRepo;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int,User> userRepo, IRepository<int, UserDetail> userDetailRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _userDetailRepo= userDetailRepo;
            _tokenService = tokenService;
        }
        
        public async Task<LoginReturnDTO> Login(UserLoginDTO loginDTO)
        {
            try
            {
                var userDb = await _userDetailRepo.GetByKey(loginDTO.UserId);
                HMACSHA512 hMACSHA = new HMACSHA512(userDb.PasswordHashKey);
                var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                bool isPasswordSame = ComparePassword(encrypterPass, userDb.Password);
                if (isPasswordSame)
                {
                    var user = await _userRepo.GetByKey(loginDTO.UserId);
                    LoginReturnDTO loginReturnDTO = MapUserToLoginReturnDTO(user);
                    return loginReturnDTO;
                }
                throw new UnauthorizedUserException("Invalid UserName or Password");
            } catch (UnauthorizedUserException) {
                throw;
            }
            catch(UserDetailRepositoryException)
            {
                throw;
            }
            catch(UserRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UnableToLoginException("Not Able to Register User at this moment", ex);
            }
        }

        private LoginReturnDTO MapUserToLoginReturnDTO(User user)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();    
            returnDTO.UserId = user.UserId;
            returnDTO.Role = user.Role;
            returnDTO.Email = user.Email;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }

        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
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
