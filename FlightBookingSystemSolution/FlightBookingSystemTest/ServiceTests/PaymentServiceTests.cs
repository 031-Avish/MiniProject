//using FlightBookingSystemAPI.Contexts;
//using FlightBookingSystemAPI.Interfaces;
//using FlightBookingSystemAPI.Models;
//using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
//using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
//using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
//using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
//using FlightBookingSystemAPI.Models.DTOs;
//using FlightBookingSystemAPI.Repositories;
//using FlightBookingSystemAPI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;
//using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
//using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
//using FlightBookingSystemAPI.Exceptions.RepositoryException;


//namespace FlightBookingSystemAPI.Tests.Services
//{
//    public class PaymentServiceTests
//    {
//        private FlightBookingContext _context;
//        private IUserBookingService _userBookingService;
//        private IRepository<int, Flight> _flightRepository;
//        private IRepository<int, Schedule> _scheduleRepository;
//        private IRepository<int, RouteInfo> _routeInfoRepository;
//        private IRepository<int, Booking> _bookingRepository;
//        private IRepository<int, BookingDetail> _bookingDetailRepository;
//        private IRepository<int, User> _userRepository;
//        private IRepository<int, Passenger> _passengerRepository;
//        private IRepository<int, UserDetail> _userDetailRepository;
//        private IRepository<int, Payment> _paymentRepository;
//        private AdminFlightService _adminFlightService;
//        private AdminRouteInfoService _adminRouteInfoService;
//        private ScheduleService _scheduleService;
//        private UserService _userService;
//        private PaymentService _paymentService;
//        private UserRegisterReturnDTO _user;
//        private BookingDTO _bookingDTO;
//        private BookingReturnDTO _bookingReturn;
//        private ScheduleReturnDTO _schedule;
//        private UserRegisterDTO _userDTO;
//        private BookingReturnDTO _booking;
//        private FlightDTO _flightDTO;
//        private FlightReturnDTO _flight;
//        private RouteInfoDTO _routeInfoDTO;
//        private RouteInfoReturnDTO _route;
//        private ScheduleDTO _scheduleDTO;

//        [SetUp]
//        public async Task Setup()
//        {
//            var options = new DbContextOptionsBuilder<FlightBookingContext>()
//                .UseSqlite("DataSource=:memory:")
//                .Options;
//            _context = new FlightBookingContext(options);
//            _context.Database.OpenConnection();
//            _context.Database.EnsureCreated();

//            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
//            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
//            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
//            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
//            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
//            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
//            ITokenService _tokenService = new TokenService(mockConfig.Object);


//            // flight repo 
//            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
//            _flightRepository = new FlightRepository(_context, flightLoggerMock.Object);

//            // schedule repo 
//            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
//            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

//            // Route Info repo 
//            var RouteLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
//            _routeInfoRepository = new RouteInfoRepository(_context, RouteLoggerMock.Object);

//            //Booking repo 
//            var bookingLoggerMock = new Mock<ILogger<BookingRepository>>();
//            _bookingRepository = new BookingRepository(_context, bookingLoggerMock.Object);

//            //userRepo 
//            var userloggerMock = new Mock<ILogger<UserRepository>>();
//            _userRepository = new UserRepository(_context, userloggerMock.Object);

//            //userDetailRepo 
//            var userDetailloggerMock = new Mock<ILogger<UserDetailRepository>>();
//            _userDetailRepository = new UserDetailRepository(_context, userDetailloggerMock.Object);

//            // booking detail repo 
//            var bookingDetailLoggerMock = new Mock<ILogger<BookingDetailRepository>>();
//            _bookingDetailRepository = new BookingDetailRepository(_context, bookingDetailLoggerMock.Object);

//            // passenger repo 
//            var passengeLoggerMock = new Mock<ILogger<PassengerRepository>>();
//            _passengerRepository = new PassengerRepository(_context, passengeLoggerMock.Object);

//            // payment repo
//            var paymentLoggerMock = new Mock<ILogger<PaymentRepository>>();
//            _paymentRepository = new PaymentRepository(_context, paymentLoggerMock.Object);

//            // payment Service 
//            var paymentLoggerMockService = new Mock<ILogger<PaymentService>>();
//            _paymentService = new PaymentService(_bookingRepository,_paymentRepository,_scheduleRepository, paymentLoggerMockService.Object);
//            // user Service 
//            var userLoggerMock1 = new Mock<ILogger<UserService>>();
//            _userService = new UserService(_userRepository, _userDetailRepository, _tokenService, userLoggerMock1.Object);

//            // flight serrvice 
//            var adminFlightserviceMock = new Mock<ILogger<AdminFlightService>>();
//            _adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, adminFlightserviceMock.Object);

//            // route service 
//            var AdminRouteInfoServiceMock = new Mock<ILogger<AdminRouteInfoService>>();
//            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, AdminRouteInfoServiceMock.Object);

//            // schedule service 
//            var ScheduleServiceMock = new Mock<ILogger<ScheduleService>>();
//            _scheduleService = new ScheduleService(_scheduleRepository, _routeInfoRepository, _flightRepository, _bookingRepository, ScheduleServiceMock.Object);

//            // booking service
//            var BookingServiceMock = new Mock<ILogger<UserBookingService>>();
//            _userBookingService = new UserBookingService(_bookingRepository, _bookingDetailRepository, _passengerRepository, _scheduleRepository, null, _context, BookingServiceMock.Object);

//        }
//        [TearDown]
//        public void TearDown()
//        {
//            _context.Database.CloseConnection();
//            _context.Dispose();
//        }
//        public async Task setup()
//        {
//            _flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            _flight = await _adminFlightService.AddFlight(_flightDTO);

//            _routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            _route = await _adminRouteInfoService.AddRouteInfo(_routeInfoDTO);

//            _scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(1),
//                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
//                Price = 20000,
//                RouteId = _route.RouteId,
//                FlightId = _flight.FlightId
//            };
//            _schedule = await _scheduleService.AddSchedule(_scheduleDTO);
//            _userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            _user = await _userService.Register(_userDTO);

//            _bookingDTO = new BookingDTO
//            {
//                UserId = _user.UserId,
//                ScheduleId = _schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };
//            _booking = await _userBookingService.BookFlight(_bookingDTO);
//        }
//        [Test]
//        public async Task ProcessPayment_Success()
//        {
//            // Arrange
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = _booking.TotalPrice,
//                BookingId = _booking.BookingId,
//            };
//            // Act 
//            var result = await _paymentService.ProcessPayment(paymentDTO);
//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(paymentDTO.Amount, result.Amount);
//            Assert.AreEqual(1, result.PaymentId);
//        }

//        [Test]
//        public async Task ProcessPayment_Failed()
//        {
//            // Arrange
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = -20,
//                BookingId = _booking.BookingId,
//            };
//            // Act 
//            var result = await _paymentService.ProcessPayment(paymentDTO);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(paymentDTO.Amount, result.Amount);
//            Assert.AreEqual(1, result.PaymentId);
//            Assert.AreEqual("Failed", result.PaymentStatus);
//        }

//        [Test]
//        public async Task ProcessPayment_BookingRepo_Exception()
//        {
//            // arrange 
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
               
//            };
            
//            // Act and Assert
//            Assert.ThrowsAsync<BookingRepositoryException>(async ()=> await _paymentService.ProcessPayment(paymentDTO));
//        }
//        [Test]
//        public async Task ProcessPayment_Exception()
//        {
//            // Arrange 
//            setup();
//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = -20,
//                BookingId = _booking.BookingId,
//            };
//            await _paymentService.ProcessPayment(paymentDTO);

//            // Act and Assert
//            Assert.ThrowsAsync<PaymentServiceException>(async()=> 
//                    await _paymentService.ProcessPayment(paymentDTO));
//        }

//        [Test]
//        public async Task GetAllPayment_Success()
//        {
//            // Arrange
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = _booking.TotalPrice,
//                BookingId = _booking.BookingId,
//            };
//            var pay = await _paymentService.ProcessPayment(paymentDTO);
//            // Act 
//            var results = await _paymentService.GetAllPayments();
//            // Assert

//            Assert.IsNotNull(results);
//            Assert.AreEqual(pay.PaymentId, results[0].PaymentId);
//        }
//        [Test]
//        public async Task GetAllPayments_Failure()
//        {
//            // Arrange
//            setup();

//            // Act
//            // Assert

//            Assert.ThrowsAsync<PaymentRepositoryException>(async () => await _paymentService.GetAllPayments());
//        }
//        [Test]
//        public async Task GetAllPayments_Exception()
//        {
//            // Arrange

//            var repositoryMock = new Mock<IRepository<int, Payment>>();
//            repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Test exception"));
//            var paymentservice = new PaymentService(_bookingRepository,repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<PaymentService>>());

//            // Act & Assert
//            Assert.ThrowsAsync<PaymentServiceException>(async () => await paymentservice.GetAllPayments());
//        }

//        [Test]
//        public async Task GetAllPaymentById_Success()
//        {
//            // Arrange
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = _booking.TotalPrice,
//                BookingId = _booking.BookingId,
//            };
//            var pay = await _paymentService.ProcessPayment(paymentDTO);
//            // Act 
//            var result = await _paymentService.GetPaymentById(pay.PaymentId);
//            // Assert

//            Assert.IsNotNull(result);
//            Assert.AreEqual(pay.PaymentId, result.PaymentId);
//        }
//        [Test]
//        public async Task GetAllPaymentById_Failure()
//        {
//            // Arrange
//            setup();

//            // Act
//            // Assert

//            Assert.ThrowsAsync<PaymentRepositoryException>(async () => await _paymentService.GetPaymentById(1234));
//        }
//        [Test]
//        public async Task GetAllPaymentById_Exception()
//        {
//            // Arrange

//            var repositoryMock = new Mock<IRepository<int, Payment>>();
//            repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Test exception"));
//            var paymentservice = new PaymentService(_bookingRepository, repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<PaymentService>>());

//            // Act & Assert
//            Assert.ThrowsAsync<PaymentServiceException>(async () => await paymentservice.GetPaymentById(1222));
//        }

//        [Test]
//        public async Task GetAllPaymentByBookingId_Success()
//        {
//            // Arrange
//            setup();

//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = _booking.TotalPrice,
//                BookingId = _booking.BookingId,
//            };
//            var pay = await _paymentService.ProcessPayment(paymentDTO);
//            // Act 
//            var result = await _paymentService.GetPaymentsByBookingId(pay.BookingId);
//            // Assert

//            Assert.IsNotNull(result);
//            Assert.AreEqual(pay.PaymentId, result[0].PaymentId);
//        }
//        [Test]
//        public async Task GetAllPaymentByBookingId_Failure()
//        {
//            // Arrange
//            setup();

//            // Act
//            // Assert

//            Assert.ThrowsAsync<PaymentRepositoryException>(async () => await _paymentService.GetPaymentsByBookingId(_booking.BookingId));
//        }
//        [Test]
//        public async Task GetAllPaymentByBookingId_Exception()
//        {
//            // Arrange

//            var repositoryMock = new Mock<IRepository<int, Payment>>();
//            repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Test exception"));
//            var paymentservice = new PaymentService(_bookingRepository, repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<PaymentService>>());

//            // Act & Assert
//            Assert.ThrowsAsync<PaymentServiceException>(async () => await paymentservice.GetPaymentsByBookingId(100));
//        }
//        [Test]
//        public async Task GetAllPaymentByBookingId_ServiceFailure()
//        {
//            // Arrange
//            setup();
//            var paymentDTO = new PaymentInputDTO
//            {
//                Amount = _booking.TotalPrice,
//                BookingId = _booking.BookingId,
//            };
//            var pay = await _paymentService.ProcessPayment(paymentDTO);

//            // Act
//            // Assert

//            Assert.ThrowsAsync<PaymentServiceException>(async () => await _paymentService.GetPaymentsByBookingId(999));
//        }
//    }
//}