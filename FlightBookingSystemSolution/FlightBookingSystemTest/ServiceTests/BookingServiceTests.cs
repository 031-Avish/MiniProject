//using FlightBookingSystemAPI.Contexts;
//using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
//using FlightBookingSystemAPI.Interfaces;
//using FlightBookingSystemAPI.Models;
//using FlightBookingSystemAPI.Models.DTOs;
//using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
//using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
//using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
//using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
//using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
//using FlightBookingSystemAPI.Repositories;
//using FlightBookingSystemAPI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;


//namespace FlightBookingSystemAPI.Tests.Services
//{
//    public class UserBookingServiceTests
//    {
//        private FlightBookingContext _context;
//        private IUserBookingService _userBookingService;
//        private IRepository<int, Flight> _flightRepository;
//        private IRepository<int, Schedule> _scheduleRepository;
//        private IRepository<int, RouteInfo> _routeInfoRepository;
//        private IRepository<int, Booking> _bookingRepository;
//        private IRepository<int, BookingDetail> _bookingDetailRepository;
//        private IRepository<int,User> _userRepository;
//        private IRepository<int, Passenger> _passengerRepository;
//        private IRepository<int,UserDetail> _userDetailRepository;
//        private AdminFlightService _adminFlightService;
//        private AdminRouteInfoService _adminRouteInfoService;
//        private ScheduleService _scheduleService;
//        private UserService _userService;

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

//        [Test]
//        public async Task BookFlight_Success()
//        {

//            // Arrange
//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(1),
//                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };

//            // Act
//            var result = await _userBookingService.BookFlight(bookingDTO);

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual("Processing", result.BookingStatus);
//            Assert.Greater(result.TotalPrice, 0);
//            Assert.AreEqual(2, result.Passengers.Count);
//            Assert.AreEqual(bookingDTO.UserId, result.UserId);
//            Assert.AreEqual(bookingDTO.ScheduleId, result.ScheduleId);
//        }

//        [Test]
//        public async Task BookFlight_Fail_OldSchedule()
//        {

//            // Arrange
//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now,
//                ReachingTime = DateTime.Now.AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };

//            // Act and Assert
//            Assert.ThrowsAsync<BookingServiceException>(async ()=> await _userBookingService.BookFlight(bookingDTO));
//        }

//        [Test]
//        public async Task BookFlight_Failure_NotEnoughSeats()
//        {
//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 1
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(1),
//                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };

//            // Act & Assert
//            Assert.ThrowsAsync<BookingServiceException>(async () => await _userBookingService.BookFlight(bookingDTO));
//        }
//        [Test]
//        public async Task CancelBooking_Success()
//        {
//            // Arrange
//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(1),
//                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);

//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);
//            // Act
//            var result = await _userBookingService.CancelBooking(booking.BookingId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(booking.BookingId, result.BookingId);
//            Assert.AreEqual("Cancellation successful. Refund will be credited in the next 3 days.", result.Message);
//            Assert.Greater(result.RefundAmount, 0);
//        }

//        [Test]
//        public async Task CancelBooking_Fail_InvalidState()
//        {
//            // Arrange
//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(1),
//                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);

           
//            // Act
//            // Assert
//            Assert.ThrowsAsync<BookingServiceException> (async ()=>await _userBookingService.CancelBooking(booking.BookingId));
//        }
//        [Test]
//        public async Task GetOldFlightsByUser_Success()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddHours(1), // An old departure time
//                ReachingTime = DateTime.Now.AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            // Booking an old flight
//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//                {
//                    new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//                    new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//                }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);

//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);

//            // Act
//            var result = await _userBookingService.GetOldFlightsByUser(user.UserId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(0, result.Count);
          
//        }

//        [Test]
//        public async Task GetOldFlightsByUser_EmptyResult()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            // Act and assert
//           Assert.ThrowsAsync<BookingServiceException>(async ()=> await _userBookingService.GetOldFlightsByUser(user.UserId));

//        }
//        [Test]
//        public async Task GetUpcomingFlightsByUser_Success()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(2),
//                ReachingTime = DateTime.Now.AddDays(2).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            // Booking an upcoming flight
//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//        {
//            new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//            new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//        }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);
//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);
//            // Act
//            var result = await _userBookingService.GetUpcomingFlightsByUser(user.UserId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(1, result.Count);
//            // Add other assertions for properties as per your implementation
//        }

//        [Test]
//        public async Task GetUpcomingFlightsByUser_EmptyResult()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            // Act and Assert
//            Assert.ThrowsAsync<BookingServiceException>(async () => await _userBookingService.GetUpcomingFlightsByUser(user.UserId));

//        }
//        [Test]
//        public async Task GetBookingDetails_ExistingBooking_Success()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(2), // An upcoming departure time
//                ReachingTime = DateTime.Now.AddDays(2).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//        {
//            new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//            new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//        }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);
//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);
            
//            // Act
//            var result = await _userBookingService.GetBookingDetails(booking.BookingId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(booking.BookingId, result.BookingId);
//            Assert.AreEqual(booking.UserId, result.UserId);
//            Assert.AreEqual(booking.ScheduleId, result.ScheduleId);
//            // Add other assertions for properties as per your implementation
//        }

//        [Test]
//        public async Task GetBookingDetails_NonExistingBooking_ThrowsException()
//        {
//            // Arrange
//            var nonExistingBookingId = 9999; // Assuming a non-existing booking ID

//            // Act & Assert
//            Assert.ThrowsAsync<BookingServiceException>(async () => await _userBookingService.GetBookingDetails(nonExistingBookingId));
//        }

//        [Test]
//        public async Task GetAllBookingsByAdmin_Success()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(2), // An upcoming departure time
//                ReachingTime = DateTime.Now.AddDays(2).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//        {
//            new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//            new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//        }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);
//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);
//            // Act

//            var result = await _userBookingService.GetAllBookingsByAdmin();

//            // Assert
//            Assert.NotNull(result);
//            Assert.GreaterOrEqual(result.Count, 1);
//            // Add other assertions for properties as per your implementation
//        }

//        [Test]
//        public async Task GetAllBookingsByAdmin_EmptyList()
//        {
//            // Arrange
            

//            // Act

//            // Assert
//           Assert.ThrowsAsync<BookingServiceException>(async () => await _userBookingService.GetAllBookingsByAdmin());
            
//        }
//        [Test]
//        public async Task GetAllBookingsByUser_Success()
//        {
//            // Arrange
//            var userDTO = new UserRegisterDTO
//            {
//                Name = "Avish",
//                Email = "avis@gmail.com",
//                Password = "password",
//                Phone = "9876543210"
//            };
//            var user = await _userService.Register(userDTO);

//            var flightDTO = new FlightDTO
//            {
//                Name = "Test Flight",
//                TotalSeats = 150
//            };
//            var flight = await _adminFlightService.AddFlight(flightDTO);

//            var routeInfoDTO = new RouteInfoDTO
//            {
//                StartCity = "City A",
//                EndCity = "City B",
//                Distance = 1000
//            };
//            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

//            var scheduleDTO = new ScheduleDTO
//            {
//                DepartureTime = DateTime.Now.AddDays(2),
//                ReachingTime = DateTime.Now.AddDays(2).AddHours(2),
//                Price = 20000,
//                RouteId = route.RouteId,
//                FlightId = flight.FlightId
//            };
//            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

//            var bookingDTO = new BookingDTO
//            {
//                UserId = user.UserId,
//                ScheduleId = schedule.ScheduleId,
//                Passengers = new List<PassengerDTO>
//        {
//            new PassengerDTO { Name = "Passenger 1", Age = 30, Gender = "Male" },
//            new PassengerDTO { Name = "Passenger 2", Age = 25, Gender = "Female" }
//        }
//            };
//            var booking = await _userBookingService.BookFlight(bookingDTO);
//            var booking1 = await _bookingRepository.GetByKey(booking.BookingId);
//            booking1.BookingStatus = "Completed";
//            booking1.PaymentStatus = "Success";
//            await _bookingRepository.Update(booking1);
//            // Act
//            var result = await _userBookingService.GetAllBookingsByUser(user.UserId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.GreaterOrEqual(result.Count, 1);
//            // Add other assertions for properties as per your implementation
//        }

//        [Test]
//        public async Task GetAllBookingsByUser_Exception()
//        {
//            // Arrange
//            var userId = 123;
//            Assert.ThrowsAsync<BookingServiceException>(async () => await _userBookingService.GetAllBookingsByUser(userId));
//        }

//    }
//}
