using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.BookingDTO;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.PassengerDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using FlightBookingSystemAPI.Repositories;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Tests.Services
{
    public class ScheduleServiceTests
    {
        private FlightBookingContext _context;
        private IUserBookingService _userBookingService;
        private IRepository<int, Flight> _flightRepository;
        private IRepository<int, Schedule> _scheduleRepository;
        private IRepository<int, RouteInfo> _routeInfoRepository;
        private IRepository<int, Booking> _bookingRepository;
        private IRepository<int, BookingDetail> _bookingDetailRepository;
        private IRepository<int, Passenger> _passengerRepository;
        private AdminFlightService _adminFlightService;
        private AdminRouteInfoService _adminRouteInfoService;
        private FlightRepository _flightRoute;
        private ScheduleService _scheduleService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new FlightBookingContext(options);
            // flight repo 
            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
            _flightRepository = new FlightRepository(_context, flightLoggerMock.Object);

            // schedule repo 
            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

            // Route Info repo 
            var RouteLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
            _routeInfoRepository = new RouteInfoRepository(_context, RouteLoggerMock.Object);

            //Booking repo 
            var bookingLoggerMock = new Mock<ILogger<BookingRepository>>();
            _bookingRepository = new BookingRepository(_context, bookingLoggerMock.Object);

            // booking detail repo 
            var bookingDetailLoggerMock = new Mock<ILogger<BookingDetailRepository>>();
            _bookingDetailRepository = new BookingDetailRepository(_context, bookingDetailLoggerMock.Object);

            // passenger repo 
            var passengeLoggerMock = new Mock<ILogger<PassengerRepository>>();
            _passengerRepository = new PassengerRepository(_context, passengeLoggerMock.Object);

            // flight serrvice 
            var adminFlightserviceMock = new Mock<ILogger<AdminFlightService>>();
            _adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, adminFlightserviceMock.Object);

            // route service 
            var AdminRouteInfoServiceMock = new Mock<ILogger<AdminRouteInfoService>>();
            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, AdminRouteInfoServiceMock.Object);

            // schedule service 
            var ScheduleServiceMock = new Mock<ILogger<ScheduleService>>();
            _scheduleService = new ScheduleService(_scheduleRepository, _routeInfoRepository, _flightRepository, _bookingRepository, ScheduleServiceMock.Object);

            // booking service
            var BookingServiceMock = new Mock<ILogger<UserBookingService>>();
            _userBookingService = new UserBookingService(_bookingRepository, _bookingDetailRepository, _passengerRepository, _scheduleRepository, null, _context, BookingServiceMock.Object);

        }

        [Test]
        public async Task BookFlight_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var schedule = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act
            var result = await _scheduleService.AddSchedule(schedule);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flight.FlightId, result.FlightId);
        }

        [Test]
        public async Task AddSchedule_Failure_ScheduleAlreadyExistsOnSameDateAndTimeRange()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var schedule = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            await _scheduleService.AddSchedule(schedule);
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight 1",
                TotalSeats = 150
            };
            var flight1 = await _adminFlightService.AddFlight(flightDTO1);
            var schedule1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.AddSchedule(schedule1));
        }

        [Test]
        public async Task AddSchedule_Failure_ScheduleWithin2HoursOfPreviousEndCityDifferent()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var schedule = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(1),
                ReachingTime = DateTime.Now.AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            await _scheduleService.AddSchedule(schedule);
           
            var schedule1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(3),
                ReachingTime = DateTime.Now.AddHours(4),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.AddSchedule(schedule1));
        }

        [Test]
        public async Task AddSchedule_Failure_RouteRepositoryException()
        {

            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);
            var schedule1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(3),
                ReachingTime = DateTime.Now.AddHours(4),
                Price = 20000,
                RouteId = 10,
                FlightId = flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _scheduleService.AddSchedule(schedule1));
          
        }

        [Test]
        public async Task AddSchedule_Failure_FlightRepositoryException()
        {

            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
            var schedule1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(3),
                ReachingTime = DateTime.Now.AddHours(4),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = 10
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _scheduleService.AddSchedule(schedule1));

        }

        [Test]
        public async Task UpdateSchedule_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);

            var updatedScheduleDTO = new ScheduleUpdateDTO
            {
                ScheduleId = addedSchedule.ScheduleId,
                DepartureTime = DateTime.Now.AddDays(2),
                ReachingTime = DateTime.Now.AddDays(2).AddHours(2),
                Price = 25000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act
            var updatedSchedule = await _scheduleService.UpdateSchedule(updatedScheduleDTO);

            // Assert
            Assert.NotNull(updatedSchedule);
            Assert.AreEqual(updatedScheduleDTO.ScheduleId, updatedSchedule.ScheduleId);
            Assert.AreEqual(updatedScheduleDTO.DepartureTime, updatedSchedule.DepartureTime);
            Assert.AreEqual(updatedScheduleDTO.ReachingTime, updatedSchedule.ReachingTime);
            Assert.AreEqual(updatedScheduleDTO.Price, updatedSchedule.Price);
        }
        [Test]
        public async Task UpdateSchedule_Failure_ScheduleAlreadyExistsOnSameDateAndTimeRange()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(18),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(19),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight1 = await _adminFlightService.AddFlight(flightDTO1);
            var route1 = await _adminRouteInfoService.AddRouteInfo(new RouteInfoDTO
            {
                StartCity = "City c",
                EndCity = "City d",
                Distance = 1000
            });
            var scheduleDTO1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(14),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(15),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight1.FlightId
            };
            var addedSchedule1 = await _scheduleService.AddSchedule(scheduleDTO1);
           

            var scheduleUpdateDTO = new ScheduleUpdateDTO
            {
                ScheduleId = addedSchedule1.ScheduleId,
                DepartureTime = DateTime.Now.AddDays(1).AddHours(18),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(19),
                Price = 25000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.UpdateSchedule(scheduleUpdateDTO));
        }

        [Test]
        public async Task UpdateSchedule_Failure_ScheduleWithin2HoursOfPreviousEndCityDifferent()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(19),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(20),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight1 = await _adminFlightService.AddFlight(flightDTO1);
            var route1 = await _adminRouteInfoService.AddRouteInfo(new RouteInfoDTO
            {
                StartCity = "City c",
                EndCity = "City d",
                Distance = 1000
            });
            var scheduleDTO1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(14),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(15),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight1.FlightId
            };
            var addedSchedule1 = await _scheduleService.AddSchedule(scheduleDTO1);


            var scheduleUpdateDTO = new ScheduleUpdateDTO
            {
                ScheduleId = addedSchedule1.ScheduleId,
                DepartureTime = DateTime.Now.AddDays(1).AddHours(18),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(19),
                Price = 25000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.UpdateSchedule(scheduleUpdateDTO));

        }

        [Test]
        public async Task UpdateSchedule_Failure_RouteRepositoryException()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(1),
                ReachingTime = DateTime.Now.AddHours(2),
                Price = 20000,
                RouteId = 1,
                FlightId = flight.FlightId
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);

            var scheduleUpdateDTO = new ScheduleUpdateDTO
            {
                ScheduleId = addedSchedule.ScheduleId,
                DepartureTime = DateTime.Now.AddHours(3),
                ReachingTime = DateTime.Now.AddHours(4),
                Price = 25000,
                RouteId = 999, // Assuming this route ID doesn't exist
                FlightId = flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _scheduleService.UpdateSchedule(scheduleUpdateDTO));
        }

        [Test]
        public async Task UpdateSchedule_Failure_FlightRepositoryException()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(1),
                ReachingTime = DateTime.Now.AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = 1
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);

            var scheduleUpdateDTO = new ScheduleUpdateDTO
            {
                ScheduleId = addedSchedule.ScheduleId,
                DepartureTime = DateTime.Now.AddHours(3),
                ReachingTime = DateTime.Now.AddHours(4),
                Price = 25000,
                RouteId = route.RouteId,
                FlightId = 999 // Assuming this flight ID doesn't exist
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _scheduleService.UpdateSchedule(scheduleUpdateDTO));
        }

        [Test]
        public async Task GetAllSchedules_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            await _scheduleService.AddSchedule(scheduleDTO);

            // Act
            var result = await _scheduleService.GetAllSchedules();

            // Assert
            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.GreaterOrEqual(result.Count, 1);
            
        }

       
        [Test]
        public async Task GetAllSchedules_Failure_ScheduleRepositoryException()
        {
            // Arrange
           

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(async () => await _scheduleService.GetAllSchedules());
        }

        [Test]
        public async Task GetAllSchedules_Failure_UnexpectedException()
        {
            // Arrange
            var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
            scheduleRepositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Unexpected error"));
            var loggerMock = new Mock<ILogger<ScheduleService>>();
            var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, _routeInfoRepository, _flightRepository, _bookingRepository, loggerMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await scheduleService.GetAllSchedules());
            
        }


        [Test]
        public async Task GetSchedule_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            var addedSchedule = await _scheduleService.AddSchedule(scheduleDTO);

            // Act
            var result = await _scheduleService.GetSchedule(addedSchedule.ScheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedSchedule.ScheduleId, result.ScheduleId);
        }

        [Test]
        public async Task GetSchedule_Failure_ScheduleNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(async () => await _scheduleService.GetSchedule(999));
        }

        

        [Test]
        public async Task GetSchedule_Failure_UnexpectedException()
        {
            // Arrange
            var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
            scheduleRepositoryMock.Setup(repo => repo.GetByKey(It.IsAny<int>())).ThrowsAsync(new Exception("Unexpected error"));
            var loggerMock = new Mock<ILogger<ScheduleService>>();
            var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, _routeInfoRepository, _flightRepository, _bookingRepository, loggerMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await scheduleService.GetSchedule(100));
            
        }

        [Test]
        public async Task GetFlightDetailsOnDate_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            await _scheduleService.AddSchedule(scheduleDTO);

            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(1),
                StartCity = "City A",
                EndCity = "City B"
            };

            // Act
            var result = await _scheduleService.GetFlightDetailsOnDate(searchDTO);

            // Assert
            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(searchDTO.StartCity, result[0].RouteInfo.StartCity);
            Assert.AreEqual(searchDTO.EndCity, result[0].RouteInfo.EndCity);
        }

        [Test]
        public async Task GetFlightDetailsOnDate_Failure_NoSchedulesFound()
        {
            // Arrange
            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(1),
                StartCity = "Nonexistent City",
                EndCity = "Another City"
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(async () => await _scheduleService.GetFlightDetailsOnDate(searchDTO));
        }

        

        [Test]
        public async Task GetFlightDetailsOnDate_Failure_UnexpectedException()
        {
            // Arrange
            var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
            scheduleRepositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Unexpected error"));
            var loggerMock = new Mock<ILogger<ScheduleService>>();
            var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, _routeInfoRepository, _flightRepository, _bookingRepository, loggerMock.Object);

            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(1),
                StartCity = "City A",
                EndCity = "City B"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ScheduleServiceException>(async () => await scheduleService.GetFlightDetailsOnDate(searchDTO));
            Assert.AreEqual("Error occurred while getting flight details on the specified date.", exception.Message);
        }
        [Test]
        public async Task GetConnectingFlights_Success()
        {
            // Arrange
            var flightDTO1 = new FlightDTO { Name = "Flight 1", TotalSeats = 150 };
            var flight1 = await _adminFlightService.AddFlight(flightDTO1);

            var flightDTO2 = new FlightDTO { Name = "Flight 2", TotalSeats = 150 };
            var flight2 = await _adminFlightService.AddFlight(flightDTO2);

            var routeInfoDTO1 = new RouteInfoDTO { StartCity = "City A", EndCity = "City B", Distance = 500 };
            var route1 = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO1);

            var routeInfoDTO2 = new RouteInfoDTO { StartCity = "City B", EndCity = "City C", Distance = 500 };
            var route2 = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO2);

            var scheduleDTO1 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(8),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(10),
                Price = 15000,
                RouteId = route1.RouteId,
                FlightId = flight1.FlightId
            };
            await _scheduleService.AddSchedule(scheduleDTO1);

            var scheduleDTO2 = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1).AddHours(12),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(13),
                Price = 15000,
                RouteId = route2.RouteId,
                FlightId = flight2.FlightId
            };
            await _scheduleService.AddSchedule(scheduleDTO2);

            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(2),
                StartCity = "City A",
                EndCity = "City C"
            };

            // Act
            var result = await _scheduleService.GetConnectingFlights(searchDTO);

            // Assert
            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result[0].Count);
            Assert.AreEqual("City A", result[0][0].RouteInfo.StartCity);
            Assert.AreEqual("City C", result[0][1].RouteInfo.EndCity);
        }

        [Test]
        public async Task GetConnectingFlights_Failure_NoConnectingFlightsFound()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            await _scheduleService.AddSchedule(scheduleDTO);
            // Arrange
            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(1),
                StartCity = "City A",
                EndCity = "City D"
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.GetConnectingFlights(searchDTO));
            
        }

       

        [Test]
        public async Task GetConnectingFlights_Failure_UnexpectedException()
        {
            // Arrange
            var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
            scheduleRepositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Unexpected error"));
            var loggerMock = new Mock<ILogger<ScheduleService>>();
            var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, _routeInfoRepository, _flightRepository, _bookingRepository, loggerMock.Object);

            var searchDTO = new ScheduleSearchDTO
            {
                Date = DateTime.Now.AddDays(1),
                StartCity = "City A",
                EndCity = "City C"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ScheduleServiceException>(async () => await scheduleService.GetConnectingFlights(searchDTO));
            Assert.AreEqual("Error occurred while getting connecting flights.Unexpected error", exception.Message);
        }
        [Test]
        public async Task DeleteSchedule_Success_NoBookingsPresent()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flight = await _adminFlightService.AddFlight(flightDTO);

            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 1000
            };
            var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            var scheduleDTO = new ScheduleDTO
            {
                DepartureTime = DateTime.Now.AddHours(1),
                ReachingTime = DateTime.Now.AddHours(2),
                Price = 20000,
                RouteId = route.RouteId,
                FlightId = flight.FlightId
            };
            var schedule = await _scheduleService.AddSchedule(scheduleDTO);

            // Act
            var result = await _scheduleService.DeleteSchedule(schedule.ScheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(schedule.ScheduleId, result.ScheduleId);
            Assert.AreEqual(flight.FlightId, result.FlightId);

        }

        // can't test because of in-memory db 
        //[Test]
        //public async Task DeleteSchedule_Success_DisableScheduleWithOldBookings()
        //{
        //    // Arrange
        //    var flightDTO = new FlightDTO
        //    {
        //        Name = "Test Flight",
        //        TotalSeats = 150
        //    };
        //    var flight = await _adminFlightService.AddFlight(flightDTO);

        //    var routeInfoDTO = new RouteInfoDTO
        //    {
        //        StartCity = "City A",
        //        EndCity = "City B",
        //        Distance = 1000
        //    };
        //    var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

        //    var scheduleDTO = new ScheduleDTO
        //    {
        //        DepartureTime = DateTime.Now,
        //        ReachingTime = DateTime.Now.AddHours(1),
        //        Price = 20000,
        //        RouteId = route.RouteId,
        //        FlightId = flight.FlightId
        //    };
        //    var schedule = await _scheduleService.AddSchedule(scheduleDTO);

        //    var bookingDTO = new BookingDTO
        //    {
        //        ScheduleId = schedule.ScheduleId,
        //        UserId = 1,
        //        Passengers = new List<PassengerDTO>
        //        {
        //            new PassengerDTO { Name = "John Doe", Age = 30, Gender = "Male" }
        //        }
        //    };
        //    await _userBookingService.BookFlight(bookingDTO);

        //    // Act
        //    var result = await _scheduleService.DeleteSchedule(schedule.ScheduleId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.AreEqual(schedule.ScheduleId, result.ScheduleId);

        //}

        //[Test]
        //public async Task DeleteSchedule_Failure_ActiveBookingsPresent()
        //{
        //    // Arrange
        //    var flightDTO = new FlightDTO
        //    {
        //        Name = "Test Flight",
        //        TotalSeats = 150
        //    };
        //    var flight = await _adminFlightService.AddFlight(flightDTO);

        //    var routeInfoDTO = new RouteInfoDTO
        //    {
        //        StartCity = "City A",
        //        EndCity = "City B",
        //        Distance = 1000
        //    };
        //    var route = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

        //    var scheduleDTO = new ScheduleDTO
        //    {
        //        DepartureTime = DateTime.Now.AddHours(1),
        //        ReachingTime = DateTime.Now.AddHours(2),
        //        Price = 20000,
        //        RouteId = route.RouteId,
        //        FlightId = flight.FlightId
        //    };
        //    var schedule = await _scheduleService.AddSchedule(scheduleDTO);

        //    var bookingDTO = new BookingDTO
        //    {
        //        ScheduleId = schedule.ScheduleId,
        //        UserId = 1,
        //        Passengers = new List<PassengerDTO>
        //{
        //    new PassengerDTO { Name = "John Doe", Age = 30, Gender = "Male" }
        //}
        //    };
        //    await _bookingService.AddBooking(bookingDTO);

        //    // Act & Assert
        //    var exception = Assert.ThrowsAsync<ScheduleServiceException>(async () => await _scheduleService.DeleteSchedule(schedule.ScheduleId));
        //    Assert.AreEqual("cannot update Schedule !! Booking has this Schedule Update that first", exception.Message);
        //}

        //[Test]
        //public async Task DeleteSchedule_Failure_BookingRepositoryException()
        //{
        //    // Arrange
        //    var bookingRepositoryMock = new Mock<IRepository<int, Booking>>();
        //    bookingRepositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new BookingRepositoryException("Error retrieving bookings"));
        //    var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
        //    var loggerMock = new Mock<ILogger<ScheduleService>>();
        //    var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, bookingRepositoryMock.Object, _routeInfoRepository, _flightRepository, loggerMock.Object);

        //    // Act & Assert
        //    Assert.ThrowsAsync<BookingRepositoryException>(async () => await scheduleService.DeleteSchedule(1));
        //}

        //[Test]
        //public async Task DeleteSchedule_Failure_ScheduleRepositoryException()
        //{
        //    // Arrange
        //    var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
        //    scheduleRepositoryMock.Setup(repo => repo.DeleteByKey(It.IsAny<int>())).ThrowsAsync(new ScheduleRepositoryException("Error deleting schedule"));
        //    var bookingRepositoryMock = new Mock<IRepository<int, Booking>>();
        //    bookingRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Booking>());
        //    var loggerMock = new Mock<ILogger<ScheduleService>>();
        //    var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, bookingRepositoryMock.Object, _routeInfoRepository, _flightRepository, loggerMock.Object);

        //    // Act & Assert
        //    Assert.ThrowsAsync<ScheduleRepositoryException>(async () => await scheduleService.DeleteSchedule(1));
        //}

        //[Test]
        //public async Task DeleteSchedule_Failure_UnexpectedException()
        //{
        //    // Arrange
        //    var scheduleRepositoryMock = new Mock<IRepository<int, Schedule>>();
        //    scheduleRepositoryMock.Setup(repo => repo.DeleteByKey(It.IsAny<int>())).ThrowsAsync(new Exception("Unexpected error"));
        //    var bookingRepositoryMock = new Mock<IRepository<int, Booking>>();
        //    bookingRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Booking>());
        //    var loggerMock = new Mock<ILogger<ScheduleService>>();
        //    var scheduleService = new ScheduleService(scheduleRepositoryMock.Object, bookingRepositoryMock.Object, _routeInfoRepository, _flightRepository, loggerMock.Object);

        //    // Act & Assert
        //    var exception = Assert.ThrowsAsync<ScheduleServiceException>(async () => await scheduleService.DeleteSchedule(1));
        //    Assert.AreEqual("Error occurred while deleting the schedule.Unexpected error", exception.Message);
        //}

    }
}