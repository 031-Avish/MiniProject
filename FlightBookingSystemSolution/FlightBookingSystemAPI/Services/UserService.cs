using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;


namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Service class responsible for user authentication and registration.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, UserDetail> _userDetailRepo;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepo">The user repository.</param>
        /// <param name="userDetailRepo">The user detail repository.</param>
        /// <param name="tokenService">The token service.</param>
        /// <param name="logger">The logger.</param>
        #region UserService Constructor 
        public UserService(
            IRepository<int, User> userRepo,
            IRepository<int, UserDetail> userDetailRepo,
            ITokenService tokenService,
            ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _userDetailRepo = userDetailRepo;
            _tokenService = tokenService;
            _logger = logger;
        }
        #endregion

        #region Login
        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="loginDTO">The login credentials.</param>
        /// <returns>The login response.</returns>
        public async Task<LoginReturnDTO> Login(UserLoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user with ID {UserId}.", loginDTO.UserId);

                // Get the user detail by ID
                var userDb = await _userDetailRepo.GetByKey(loginDTO.UserId);

                // Hash the key
                using (HMACSHA512 hMACSHA = new HMACSHA512(userDb.PasswordHashKey))
                {
                    var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

                    // Compare the password
                    bool isPasswordSame = ComparePassword(encrypterPass, userDb.Password);
                    if (isPasswordSame)
                    {
                        // Get User If Id password are correct and Map to return DTO 
                        var user = await _userRepo.GetByKey(loginDTO.UserId);
                        LoginReturnDTO loginReturnDTO = MapUserToLoginReturnDTO(user);
                        _logger.LogInformation("User with ID {UserId} logged in successfully.", loginDTO.UserId);
                        return loginReturnDTO;
                    }
                }

                _logger.LogWarning("Invalid username or password for user ID {UserId}.", loginDTO.UserId);
                throw new UnauthorizedUserException("Invalid username or password.");
            }
            catch (UnauthorizedUserException ex)
            {
                _logger.LogError(ex, "UnauthorizedUserException for user ID {UserId}.", loginDTO.UserId);
                throw;
            }
            catch (UserDetailRepositoryException ex)
            {
                _logger.LogError(ex, "UserDetailRepositoryException for user ID {UserId}.", loginDTO.UserId);
                throw;
            }
            catch (UserServiceException ex)
            {
                _logger.LogError(ex, "UserRepositoryException for user ID {UserId}.", loginDTO.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while logging in user ID {UserId}.", loginDTO.UserId);
                throw new UnableToLoginException("Not able to log in user at this moment.", ex);
            }
        }
        #endregion

        #region Register
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegisterDTO">The user registration details.</param>
        /// <returns>The registered user details.</returns>
        public async Task<UserRegisterReturnDTO> Register(UserRegisterDTO userRegisterDTO)
        {
            User user = null;
            UserDetail userDetail = null;

            try
            {
                _logger.LogInformation("Attempting to register user with email {Email}.", userRegisterDTO.Email);
                //Gemereate User and User Detail object 
                user = GenerateUser(userRegisterDTO);
                userDetail = MapUserRegisterDTOToUserDetail(userRegisterDTO);

                // add user and user Detail with hashed password 
                user = await _userRepo.Add(user);
                userDetail.UserId = user.UserId;
                userDetail = await _userDetailRepo.Add(userDetail);
                // Map to return DTO 
                UserRegisterReturnDTO userRegisterReturnDTO = MapUserToReturnDTO(user);

                _logger.LogInformation("User with email {Email} registered successfully.", userRegisterDTO.Email);
                return userRegisterReturnDTO;


            }
            catch (UserServiceException ex)
            {
                _logger.LogError(ex, "UserRepositoryException for email {Email}.", userRegisterDTO.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while registering user with email {Email}.", userRegisterDTO.Email);
                // revert in case of any exception 
                if (user != null)
                {
                    await RevertUserInsert(user);
                }

                if (userDetail != null)
                {
                    await RevertUserDetailInsert(userDetail);
                }

                throw new UnableToRegisterException("Not able to register user at this moment.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Map User To Login Return DTO 
        /// </summary>
        /// <param name="user">User Object</param>
        /// <returns>Login Return DTO object </returns>
        #region MapUserToLoginReturnDTO
        private LoginReturnDTO MapUserToLoginReturnDTO(User user)
        {
            var returnDTO = new LoginReturnDTO
            {
                UserId = user.UserId,
                Role = user.Role,
                Email = user.Email,
                Token = _tokenService.GenerateToken(user)
            };
            return returnDTO;
        }

        #endregion

        /// <summary>
        /// Compare the Hashed Passwords
        /// </summary>
        /// <param name="encrypterPass">Encrypted password From Db </param>
        /// <param name="password">User Given Password </param>
        /// <returns></returns>
        #region Compare Password
        [ExcludeFromCodeCoverage]
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
        #endregion

        /// <summary>
        /// Map User To Return DTO 
        /// </summary>
        /// <param name="user">User Object</param>
        /// <returns>RUser Register Return DTO</returns>

        #region Map User To Return DTO 
        private UserRegisterReturnDTO MapUserToReturnDTO(User user)
        {
            var returnDTO = new UserRegisterReturnDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone
            };
            return returnDTO;
        }
        #endregion

        #region Revert Changes 
        /// <summary>
        /// Revert the User Insert 
        /// </summary>
        /// <param name="userDetail"> UserDetail Object </param>
        /// <returns>Void </returns>
        private async Task RevertUserDetailInsert(UserDetail userDetail)
        {
            _logger.LogWarning("Reverting user detail insert for user ID {UserId}.", userDetail.UserId);
            await _userDetailRepo.DeleteByKey(userDetail.UserId);
        }
        /// <summary>
        /// Revert the User Insert 
        /// </summary>
        /// <param name="user"> User Object </param>
        /// <returns>Void </returns>
        private async Task RevertUserInsert(User user)
        {
            _logger.LogWarning("Reverting user insert for user ID {UserId}.", user.UserId);
            await _userRepo.DeleteByKey(user.UserId);
        }
        #endregion

        /// <summary>
        /// Make A User Detail Object with Hashed Password 
        /// </summary>
        /// <param name="userRegisterDTO"> User Register DTO object</param>
        /// <returns>User Detail Object </returns>
        #region Map User Register DTO to User Detail 
        private UserDetail MapUserRegisterDTOToUserDetail(UserRegisterDTO userRegisterDTO)
        {
            using (HMACSHA512 hMACSHA512 = new HMACSHA512())
            {
                var userDetail = new UserDetail
                {
                    PasswordHashKey = hMACSHA512.Key,
                    Email = userRegisterDTO.Email,
                    Password = hMACSHA512.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.Password))
                };
                return userDetail;
            }
        }
        #endregion

        /// <summary>
        /// Generate user From User Register DTO 
        /// </summary>
        /// <param name="userRegisterDTO">User Register Dto object </param>
        /// <returns>User Object </returns>
        #region GenerateUser 
        private User GenerateUser(UserRegisterDTO userRegisterDTO)
        {
            var user = new User
            {
                Name = userRegisterDTO.Name,
                Email = userRegisterDTO.Email,
                Phone = userRegisterDTO.Phone
            };
            return user;
        }
        #endregion
    }
}
