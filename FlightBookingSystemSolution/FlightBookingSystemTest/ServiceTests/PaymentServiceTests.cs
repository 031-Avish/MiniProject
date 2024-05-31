//using FlightBookingSystemAPI.Contexts;
//using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
//using FlightBookingSystemAPI.Interfaces;
//using FlightBookingSystemAPI.Models;
//using FlightBookingSystemAPI.Models.DTOs.PaymentDTOs;
//using FlightBookingSystemAPI.Repositories;
//using FlightBookingSystemAPI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace FlightBookingSystemAPI.Tests.Services
//{
//    public class PaymentServiceTests
//    {
//        //private IPaymentService _paymentService;
//        private FlightBookingContext _context;
//        private IRepository<int,Flight> _flightRepository;
//        private IRepository<int, Booking> _bookingRepository;
//        private IRepository<int, Payment> _paymentRepository;
//        private PaymentService _paymentService;
//        private IRepository<int, Schedule> _scheduleRepository;
//        private ILogger<PaymentService> _logger;
//        private IRepository<int, RouteInfo> _routeInfoRepository;
//        private AdminRouteInfoService _adminRouteInfoService;
//        private AdminFlightService _adminFlightService;

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

//            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
//            _flightRepository = new FlightRepository(_context, flightLoggerMock.Object);

//            var BookingLoggerMock  = new Mock<ILogger<BookingRepository>>();
//            _bookingRepository = new BookingRepository(_context, BookingLoggerMock.Object);

//            var paymentLoggerMock = new Mock<ILogger<PaymentRepository>>();
//            _paymentRepository = new PaymentRepository(_context, paymentLoggerMock.Object);

//            var paymentLoggerMock1 = new Mock<ILogger<PaymentService>>();
//            _paymentService = new PaymentService(
//                _bookingRepository,
//                _paymentRepository,
//                _scheduleRepository,
//                paymentLoggerMock1.Object
//                );
//            var adminLoggerMock = new Mock<ILogger<AdminRouteInfoService>>();
//            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, adminLoggerMock.Object);
//            var adminLoggerMock1 = new Mock<ILogger<AdminFlightService>>();
//            _adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, adminLoggerMock1.Object);
//            var bookingLoggerMock = new Mock<ILogger<UserBookingService>>();
//            _bookingService = new UserBookingService(_bookingRepository,_paymentRepository, _scheduleRepository, _userRepository , bookingLoggerMock.Object);
//        }

//        [Test]
//        public async Task ProcessPayment_Success()
//        {
//            // Arrange
//            var route = new RouteInfo
//            {
//                StartCity = "CityA",
//                EndCity = "CityB",
//                Distance = 1000
//            };
//            route = await _routeInfoRepository.Add(route);
//            var flight = new Flight
//            {
//                Name = "Test Flight",
//                TotalSeats = 200 
//            };

//            flight = await _flightRepository.Add(flight);

//            var schedule = new Schedule
//            {
//                DepartureTime = DateTime.Now.AddHours(1),
//                ReachingTime = DateTime.Now.AddHours(3),
//                AvailableSeat = flight.TotalSeats, 
//                ScheduleStatus = "Enable", 
//                Price = 100,
//                FlightId = flight.FlightId,
//                RouteId=route.RouteId
//            };

//            var booking = new Booking
//            {
//                ScheduleId = schedule.ScheduleId,
//                BookingStatus = "Processing",
//                PaymentStatus = "Pending",
//                TotalPrice = 20000,

//            };

//            await _context.Bookings.AddAsync(booking);
//            await _context.SaveChangesAsync();

//            var paymentInputDTO = new PaymentInputDTO
//            {
//                Amount = 100, // Example amount
//                BookingId = booking.BookingId
//            };

//            // Act
//            var result = await _paymentService.ProcessPayment(paymentInputDTO);

//            // Assert
//            Assert.IsNotNull(result);
//            // Add more assertions based on expected behavior
//        }

//        // Add more test cases to cover other scenarios like payment failure, repository exceptions, etc.
//    }
//}
