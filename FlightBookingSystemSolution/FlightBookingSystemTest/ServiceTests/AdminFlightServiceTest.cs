using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Repositories;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightBookingSystemAPI.Tests.Services
{
    public class AdminFlightServiceTests
    {
        private FlightBookingContext _context;
        private IRepository<int, Flight> _flightRepository;
        private IRepository<int, Schedule> _scheduleRepository;
        private AdminFlightService _adminFlightService;
        private IRepository<int,RouteInfo> _routeInfoRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new FlightBookingContext(options);

            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
            _flightRepository = new FlightRepository(_context, flightLoggerMock.Object);

            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);
            var RouteLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
            _routeInfoRepository = new RouteInfoRepository(_context, RouteLoggerMock.Object);
            var adminLoggerMock = new Mock<ILogger<AdminFlightService>>();
            _adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, adminLoggerMock.Object);
        }

        [Test]
        public async Task AddFlight_Success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };

            // Act
            var result = await _adminFlightService.AddFlight(flightDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flightDTO.Name, result.Name);
            Assert.AreEqual(flightDTO.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void AddFlight_Failure_FlightRepositoryException()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = null
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async() => await _adminFlightService.AddFlight(flightDTO));
        }

        [Test]
        public void AddFlight_Failure_AdminFlightServiceException()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };

            // Mocking an unexpected error scenario
            _flightRepository = new Mock<IRepository<int, Flight>>().Object;
            var adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, Mock.Of<ILogger<AdminFlightService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await  adminFlightService.AddFlight(flightDTO));
        }

        [Test]  
        public async Task DeleteFlight_success()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var addedFlight = await _adminFlightService.AddFlight(flightDTO);

            // Act
            var result = await _adminFlightService.DeleteFlight(addedFlight.FlightId);

            // Assert 
            Assert.NotNull(result);
            Assert.AreEqual(addedFlight.FlightId, result.FlightId);
            Assert.AreEqual(addedFlight.Name, result.Name);
            Assert.AreEqual(addedFlight.TotalSeats, result.TotalSeats);
        }
        [Test]
        public async Task DeleteFlight_Success_NoSchedule()
        {
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight1",
                TotalSeats = 120
            };
            var addedFlight1= await _adminFlightService.AddFlight(flightDTO1);
            var addedFlight = await _adminFlightService.AddFlight(flightDTO);
            var RouteInfo = new RouteInfo
            {
                StartCity = "Chennai",
                EndCity = "Mumbai",
                Distance = 1000,
            };
            var route= await _routeInfoRepository.Add(RouteInfo);

            var schedule = new Schedule
            {
                DepartureTime = new DateTime(2024, 6, 10, 1, 29, 46, 798),
                ReachingTime = new DateTime(2024, 6, 10, 3, 29, 46, 798),
                AvailableSeat = 150,
                ScheduleStatus = "Enable",
                Price = 20000,
                FlightId = addedFlight1.FlightId,
                RouteId = route.RouteId
            };
            await _scheduleRepository.Add(schedule);


            var result = await _adminFlightService.DeleteFlight(addedFlight.FlightId);

            // Assert 
            Assert.NotNull(result);
            Assert.AreEqual(addedFlight.FlightId, result.FlightId);
            Assert.AreEqual(addedFlight.Name, result.Name);
            Assert.AreEqual(addedFlight.TotalSeats, result.TotalSeats);
        }
        [Test]
        public async Task DeleteFlight_AdminFlightServiceException()
        {
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight1",
                TotalSeats = 120
            };
            var addedFlight1 = await _adminFlightService.AddFlight(flightDTO1);
            //var addedFlight = await _adminFlightService.AddFlight(flightDTO);
            var RouteInfo = new RouteInfo
            {
                StartCity = "Chennai",
                EndCity = "Mumbai",
                Distance = 1000,
            };
            var route = await _routeInfoRepository.Add(RouteInfo);

            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                AvailableSeat = 150,
                ScheduleStatus = "Enable",
                Price = 20000,
                FlightId = addedFlight1.FlightId,
                RouteId = route.RouteId
            };
            await _scheduleRepository.Add(schedule);
            // Assert 
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await _adminFlightService.DeleteFlight(addedFlight1.FlightId));
        }
        [Test]
        public async Task DeleteFlight_FlightRepositoryException()
        {
            // act  and Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _adminFlightService.DeleteFlight(20));
        }
        [Test]  
        public async Task DeleteFlight_Failure_AdminFlightServiceException()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };

            var result = await _adminFlightService.AddFlight(flightDTO);
            // Mocking an unexpected error scenario
            _flightRepository = new Mock<IRepository<int, Flight>>().Object;
            var adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository, Mock.Of<ILogger<AdminFlightService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await adminFlightService.DeleteFlight(result.FlightId));
        }
        [Test]
        public async Task DeleteFlight_Success_OldSchedule()
        {
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            var flightDTO1 = new FlightDTO
            {
                Name = "Test Flight1",
                TotalSeats = 120
            };
            var addedFlight1 = await _adminFlightService.AddFlight(flightDTO1);
            var addedFlight = await _adminFlightService.AddFlight(flightDTO);
            var RouteInfo = new RouteInfo
            {
                StartCity = "Chennai",
                EndCity = "Mumbai",
                Distance = 1000,
            };
            var route = await _routeInfoRepository.Add(RouteInfo);

            var schedule = new Schedule
            {
                DepartureTime = new DateTime(2024, 5, 10, 1, 29, 46, 798),
                ReachingTime = new DateTime(2024, 5, 10, 3, 29, 46, 798),
                AvailableSeat = 150,
                ScheduleStatus = "Enable",
                Price = 20000,
                FlightId = addedFlight.FlightId,
                RouteId = route.RouteId
            };
            await _scheduleRepository.Add(schedule);


            var result = await _adminFlightService.DeleteFlight(addedFlight.FlightId);

            // Assert 
            Assert.NotNull(result);
            Assert.AreEqual(addedFlight.FlightId, result.FlightId);
            Assert.AreEqual(addedFlight.Name, result.Name);
            Assert.AreEqual(addedFlight.TotalSeats, result.TotalSeats);
        }
        [Test]
        public async Task GetAllFlight_Success()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight {  Name = "Flight 1", TotalSeats = 100 },
                new Flight {  Name = "Flight 2", TotalSeats = 150 }
            };
            await _context.Flights.AddRangeAsync(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminFlightService.GetAllFlight();

            // Assert
            Assert.NotNull(result);
            Assert.GreaterOrEqual(result.Count, 2);
        }

        [Test]
        public void GetAllFlight_Failure_FlightRepositoryException()
        {
            // Arrange
            
            // there is nothing added 

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _adminFlightService.GetAllFlight());
        }

        [Test]
        public void GetAllFlight_Failure_AdminFlightServiceException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<int, Flight>>();
            repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Test exception"));
            var adminFlightService = new AdminFlightService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminFlightService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await adminFlightService.GetAllFlight());
        }

        [Test]
        public async Task GetFlight_Success()
        {
            // Arrange
            var flight = new Flight
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminFlightService.GetFlight(flight.FlightId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flight.FlightId, result.FlightId);
            Assert.AreEqual(flight.Name, result.Name);
            Assert.AreEqual(flight.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void GetFlight_Failure_FlightRepositoryException()
        {
            // Arrange
            var flightId = 999; // Assuming this ID does not exist in the database

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _adminFlightService.GetFlight(flightId));
        }

        [Test]
        public void GetFlight_Failure_AdminFlightServiceException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<int, Flight>>();
            repositoryMock.Setup(repo => repo.GetByKey(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));
            var adminFlightService = new AdminFlightService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminFlightService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await adminFlightService.GetFlight(1));
        }
        [Test]
        public async Task UpdateFlight_Success()
        {
            // Arrange
            var flight = new Flight
            {
                Name = "Test Flight",
                TotalSeats = 150
            };
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            var flightReturnDTO = new FlightReturnDTO
            {
                FlightId = flight.FlightId,
                Name = "Updated Flight",
                TotalSeats = 200
            };

            // Act
            var result = await _adminFlightService.UpdateFlight(flightReturnDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flightReturnDTO.FlightId, result.FlightId);
            Assert.AreEqual(flightReturnDTO.Name, result.Name);
            Assert.AreEqual(flightReturnDTO.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void UpdateFlight_Failure_FlightRepositoryException()
        {
            // Arrange
            var flightReturnDTO = new FlightReturnDTO
            {
                FlightId = 999, // Assuming this ID does not exist in the database
                Name = "Updated Flight",
                TotalSeats = 200
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(async () => await _adminFlightService.UpdateFlight(flightReturnDTO));
        }

        [Test]
        public void UpdateFlight_Failure_AdminFlightServiceException()
        {
            // Arrange
            var flightReturnDTO = new FlightReturnDTO
            {
                FlightId = 1,
                Name = "Updated Flight",
                TotalSeats = 200
            };
            var repositoryMock = new Mock<IRepository<int, Flight>>();
            repositoryMock.Setup(repo => repo.Update(It.IsAny<Flight>())).ThrowsAsync(new Exception("Test exception"));
            var adminFlightService = new AdminFlightService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminFlightService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(async () => await adminFlightService.UpdateFlight(flightReturnDTO));
        }



    }
}
