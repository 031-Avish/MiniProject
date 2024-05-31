


using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Repositories;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightBookingSystemAPI.Tests.Services
{
    public class UserServiceTests
    {
        private FlightBookingContext _context;
        private IRepository<int, RouteInfo> _routeInfoRepository;
        private IRepository<int, Schedule> _scheduleRepository;
        private AdminRouteInfoService _adminRouteInfoService;
        private IRepository<int, Flight> _flightRoute;
        private IRepository<int, User> _userRepository;
        private IRepository<int, UserDetail> _userDetailRepository;
        private UserService _userService;
        private ITokenService _tokenService;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new FlightBookingContext(options);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);

            var routeLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
            _routeInfoRepository = new RouteInfoRepository(_context, routeLoggerMock.Object);

            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

            var adminLoggerMock = new Mock<ILogger<AdminRouteInfoService>>();
            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, adminLoggerMock.Object);

            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
            _flightRoute = new FlightRepository(_context, flightLoggerMock.Object);

            //userRepo 
            var userloggerMock = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, userloggerMock.Object);

            //userDetailRepo 
            var userDetailloggerMock = new Mock<ILogger<UserDetailRepository>>();
            _userDetailRepository = new UserDetailRepository(_context, userDetailloggerMock.Object);

            // user Service 
            var userLoggerMock1 = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepository, _userDetailRepository, _tokenService, userLoggerMock1.Object);


        }

        [Test]
        public async Task Register_ValidData_SuccessfullyRegistersUser()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };

            // Act
            var result = await _userService.Register(userRegisterDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRegisterDTO.Name, result.Name);
            Assert.AreEqual(userRegisterDTO.Email, result.Email);
            Assert.AreEqual(userRegisterDTO.Phone, result.Phone);
            // Add more assertions if needed
        }

        [Test]
        public async Task Register_InvalidData_ThrowsUserRepositoryException()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                // Missing required fields
                Email = "john@example.com",
                Password = "password"
            };


            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(async () => await _userService.Register(userRegisterDTO));
            
        }

        [Test]
        public async Task Register_ValidData_DuplicateEmail()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };
            var userRegisterDTO1 = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };

            // Act
            await _userService.Register(userRegisterDTO1);


            // Assert
            Assert.ThrowsAsync<UserServiceException>(async () => await _userService.Register(userRegisterDTO));
        }
        [Test]
        public async Task Register_Exception()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };


            // Act
            var repositoryMock = new Mock<IRepository<int, User>>();
            repositoryMock.Setup(repo => repo.Update(It.IsAny<User>())).ThrowsAsync(new Exception("Test exception"));
            var userMock = new UserService(repositoryMock.Object, _userDetailRepository,_tokenService, Mock.Of<ILogger<UserService>>());

            
            // Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(async () => await userMock.Register(userRegisterDTO));
        }

        [Test]
        public async Task Login_Success()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };
            var result = await _userService.Register(userRegisterDTO);

            var loginDTO = new UserLoginDTO
            {
                UserId = result.UserId,
                Password = "password"
            };
            // Act
            var res = await _userService.Login(loginDTO);
            Assert.IsNotNull(res);
            Assert.IsNotEmpty(res.Token);   
            Assert.AreEqual(result.UserId, res.UserId);
        }

        [Test]
        public async Task Login_WrongPassword()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };
            var result = await _userService.Register(userRegisterDTO);

            var loginDTO = new UserLoginDTO
            {
                UserId = result.UserId,
                Password = "sdf"
            };
            // Act and assert
            Assert.ThrowsAsync<UnauthorizedUserException> (async()=> await _userService.Login(loginDTO));
            
        }
        [Test]
        public async Task Login_NoUser()
        {
            // Arrange
            var userRegisterDTO = new UserRegisterDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "1234567890"
            };
            var result = await _userService.Register(userRegisterDTO);

            var loginDTO = new UserLoginDTO
            {
                UserId = 2,
                Password = "sdf"
            };
            // Act and assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(async () => await _userService.Login(loginDTO));

        }
    }
}
