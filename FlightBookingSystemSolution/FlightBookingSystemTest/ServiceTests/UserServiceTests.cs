


//using FlightBookingSystemAPI.Contexts;
//using FlightBookingSystemAPI.Exceptions.RepositoryException;
//using FlightBookingSystemAPI.Interfaces;
//using FlightBookingSystemAPI.Models;
//using FlightBookingSystemAPI.Models.DTOs;
//using FlightBookingSystemAPI.Repositories;
//using FlightBookingSystemAPI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace FlightBookingSystemAPI.Tests.Services
//{
//    public class UserServiceTests
//    {
//        private FlightBookingContext _context;
//        private IRepository<int, RouteInfo> _routeInfoRepository;
//        private IRepository<int, Schedule> _scheduleRepository;
//        private AdminRouteInfoService _adminRouteInfoService;
//        private FlightRepository _flightRoute;

//        [SetUp]
//        public void Setup()
//        {
//            var options = new DbContextOptionsBuilder<FlightBookingContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;
//            _context = new FlightBookingContext(options);

//            var routeLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
//            _routeInfoRepository = new RouteInfoRepository(_context, routeLoggerMock.Object);

//            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
//            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

//            var adminLoggerMock = new Mock<ILogger<AdminRouteInfoService>>();
//            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, adminLoggerMock.Object);

//            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
//            _flightRoute = new FlightRepository(_context, flightLoggerMock.Object);
//        }

//        [Test]
//public async Task Register_ValidData_SuccessfullyRegistersUser()
//{
//    // Arrange
//    var userRegisterDTO = new UserRegisterDTO
//    {
//        Name = "John Doe",
//        Email = "john@example.com",
//        Password = "password",
//        Phone = "1234567890"
//    };

//    var userService = new UserService(
//        _userRepo.Object,
//        _userDetailRepo.Object,
//        _tokenService.Object,
//        _logger.Object
//    );

//    // Act
//    var result = await userService.Register(userRegisterDTO);

//    // Assert
//    Assert.IsNotNull(result);
//    Assert.AreEqual(userRegisterDTO.Name, result.Name);
//    Assert.AreEqual(userRegisterDTO.Email, result.Email);
//    Assert.AreEqual(userRegisterDTO.Phone, result.Phone);
//    // Add more assertions if needed
//}

//[Test]
//public async Task Register_InvalidData_ThrowsUserRepositoryException()
//{
//    // Arrange
//    var userRegisterDTO = new UserRegisterDTO
//    {
//        // Missing required fields
//        Email = "john@example.com",
//        Password = "password"
//    };

//    var userService = new UserService(
//        _userRepo.Object,
//        _userDetailRepo.Object,
//        _tokenService.Object,
//        _logger.Object
//    );

//    // Act & Assert
//    var exception = Assert.ThrowsAsync<UserRepositoryException>(async () => await userService.Register(userRegisterDTO));
//    Assert.AreEqual("UserRepositoryException", exception.GetType().Name);
//}

//[Test]
//public async Task Register_UnexpectedError_ThrowsUnableToRegisterException()
//{
//    // Arrange
//    var userRegisterDTO = new UserRegisterDTO
//    {
//        Name = "John Doe",
//        Email = "john@example.com",
//        Password = "password",
//        Phone = "1234567890"
//    };

//    var userService = new UserService(
//        _userRepo.Object,
//        _userDetailRepo.Object,
//        _tokenService.Object,
//        _logger.Object
//    );

//    _userRepo.Setup(repo => repo.Add(It.IsAny<User>())).ThrowsAsync(new Exception());

//    // Act & Assert
//    var exception = Assert.ThrowsAsync<UnableToRegisterException>(async () => await userService.Register(userRegisterDTO));
//    Assert.AreEqual("UnableToRegisterException", exception.GetType().Name);
//}
