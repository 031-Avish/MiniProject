using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Repositories;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightBookingSystemAPI.Tests.Services
{
    public class AdminRouteInfoServiceTests
    {
        private FlightBookingContext _context;
        private IRepository<int, RouteInfo> _routeInfoRepository;
        private IRepository<int, Schedule> _scheduleRepository;
        private AdminRouteInfoService _adminRouteInfoService;
        private FlightRepository _flightRoute;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new FlightBookingContext(options);

            var routeLoggerMock = new Mock<ILogger<RouteInfoRepository>>();
            _routeInfoRepository = new RouteInfoRepository(_context, routeLoggerMock.Object);

            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

            var adminLoggerMock = new Mock<ILogger<AdminRouteInfoService>>();
            _adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, adminLoggerMock.Object);

            var flightLoggerMock = new Mock<ILogger<FlightRepository>>();
            _flightRoute = new FlightRepository(_context, flightLoggerMock.Object); 
        }

        [Test]
        public async Task AddRouteInfo_Success()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };

            // Act
            var result = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(routeInfoDTO.StartCity, result.StartCity);
            Assert.AreEqual(routeInfoDTO.EndCity, result.EndCity);
            Assert.AreEqual(routeInfoDTO.Distance, result.Distance);
        }

        [Test]
        public void AddRouteInfo_Failure_RouteInfoRepositoryException()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = null,
                EndCity = "City B",
                Distance = 100
            };

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _adminRouteInfoService.AddRouteInfo(routeInfoDTO));
        }

        [Test]
        public void AddRouteInfo_Failure_AdminRouteInfoServiceException()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };

            // Mocking an unexpected error scenario
            _routeInfoRepository = new Mock<IRepository<int, RouteInfo>>().Object;
            var adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, Mock.Of<ILogger<AdminRouteInfoService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async () => await adminRouteInfoService.AddRouteInfo(routeInfoDTO));
        }

        [Test]
        public async Task DeleteRouteInfo_Success()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var addedRouteInfo = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);

            // Act
            var result = await _adminRouteInfoService.DeleteRouteInfo(addedRouteInfo.RouteId);

            // Assert 
            Assert.NotNull(result);
            Assert.AreEqual(addedRouteInfo.RouteId, result.RouteId);
            Assert.AreEqual(addedRouteInfo.StartCity, result.StartCity);
            Assert.AreEqual(addedRouteInfo.EndCity, result.EndCity);
            Assert.AreEqual(addedRouteInfo.Distance, result.Distance);
        }

        [Test]
        public async Task DeleteRouteInfo_Success_NoSchedule()
        {
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var routeInfoDTO1 = new RouteInfoDTO
            {
                StartCity = "City C",
                EndCity = "City D",
                Distance = 150
            };
            
            var addedRouteInfo1 = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO1);
            var addedRouteInfo = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                AvailableSeat = 150,
                ScheduleStatus = "Enable",
                Price = 20000,
                RouteId = addedRouteInfo1.RouteId,
            };
            await _scheduleRepository.Add(schedule);


            var result = await _adminRouteInfoService.DeleteRouteInfo(addedRouteInfo.RouteId);

            // Assert 
            Assert.NotNull(result);
            Assert.AreEqual(addedRouteInfo.RouteId, result.RouteId);
            Assert.AreEqual(addedRouteInfo.StartCity, result.StartCity);
            Assert.AreEqual(addedRouteInfo.EndCity, result.EndCity);
            Assert.AreEqual(addedRouteInfo.Distance, result.Distance);
        }
        [Test]
        public async Task DeleteRouteInfo_AdminFLightServiceException()
        {
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var routeInfoDTO1 = new RouteInfoDTO
            {
                StartCity = "City C",
                EndCity = "City D",
                Distance = 150
            };
            var flight1 = new Flight
            {
                Name = "flight 1",
                TotalSeats = 120
            };
            var flight = await _flightRoute.Add(flight1);
            var addedRouteInfo1 = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO1);
            var addedRouteInfo = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                AvailableSeat = 150,
                ScheduleStatus = "Enable",
                Price = 20000,
                RouteId = addedRouteInfo.RouteId,
                FlightId= flight.FlightId
            };
            var schedule1 = await _scheduleRepository.Add(schedule);
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async () => await _adminRouteInfoService.DeleteRouteInfo(addedRouteInfo.RouteId));

        }

        [Test]
        public void DeleteRouteInfo_RouteRepositoryException()
        {
            // act  and Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _adminRouteInfoService.DeleteRouteInfo(20));
        }

        [Test]
        public async Task DeleteRouteInfo_Failure_AdminRouteInfoServiceException()
        {
            // Arrange
            var routeInfoDTO = new RouteInfoDTO
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };

            var result = await _adminRouteInfoService.AddRouteInfo(routeInfoDTO);
            // Mocking an unexpected error scenario
            _routeInfoRepository = new Mock<IRepository<int, RouteInfo>>().Object;
            var adminRouteInfoService = new AdminRouteInfoService(_routeInfoRepository, _scheduleRepository, Mock.Of<ILogger<AdminRouteInfoService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async ()=> await adminRouteInfoService.DeleteRouteInfo(result.RouteId));
        }

        [Test]
        public async Task GetAllRouteInfos_Success()
        {
            // Arrange
            var routeInfos = new List<RouteInfo>
        {
            new RouteInfo { StartCity = "City A", EndCity = "City B", Distance = 100 },
            new RouteInfo { StartCity = "City C", EndCity = "City D", Distance = 150 }
        };
            await _context.RouteInfos.AddRangeAsync(routeInfos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminRouteInfoService.GetAllRouteInfos();

            // Assert
            Assert.NotNull(result);
            Assert.GreaterOrEqual(result.Count, 2);
        }

        [Test]
        public void GetAllRouteInfos_Failure_RouteInfoRepositoryException()
        {
            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _adminRouteInfoService.GetAllRouteInfos());
        }

        [Test]
        public void GetAllRouteInfos_Failure_AdminRouteInfoServiceException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<int, RouteInfo>>();
            repositoryMock.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Test exception"));
            var adminRouteInfoService = new AdminRouteInfoService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminRouteInfoService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async () => await adminRouteInfoService.GetAllRouteInfos());
        }

        [Test]
        public async Task GetRouteInfo_Success()
        {
            // Arrange
            var routeInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            _context.RouteInfos.Add(routeInfo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminRouteInfoService.GetRouteInfo(routeInfo.RouteId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(routeInfo.RouteId, result.RouteId);
            Assert.AreEqual(routeInfo.StartCity, result.StartCity);
            Assert.AreEqual(routeInfo.EndCity, result.EndCity);
            Assert.AreEqual(routeInfo.Distance, result.Distance);
        }

        [Test]
        public void GetRouteInfo_Failure_RouteInfoRepositoryException()
        {
            // Arrange
            var routeId = 999; // Assuming this ID does not exist in the database

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _adminRouteInfoService.GetRouteInfo(routeId));
        }

        [Test]
        public void GetRouteInfo_Failure_AdminRouteInfoServiceException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<int, RouteInfo>>();
            repositoryMock.Setup(repo => repo.GetByKey(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));
            var adminRouteInfoService = new AdminRouteInfoService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminRouteInfoService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async () => await adminRouteInfoService.GetRouteInfo(1));
        }

        [Test]
        public async Task UpdateRouteInfo_Success()
        {
            // Arrange
            var routeInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            routeInfo= await _routeInfoRepository.Add(routeInfo);
            await _context.SaveChangesAsync();

            var routeInfoReturnDTO = new RouteInfoReturnDTO
            {
                RouteId = routeInfo.RouteId,
                StartCity = "Updated City A",
                EndCity = "Updated City B",
                Distance = 150,

            };

            // Act
            var result = await _adminRouteInfoService.UpdateRouteInfo(routeInfoReturnDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(routeInfoReturnDTO.RouteId, result.RouteId);
            Assert.AreEqual(routeInfoReturnDTO.StartCity, result.StartCity);
            Assert.AreEqual(routeInfoReturnDTO.EndCity, result.EndCity);
            Assert.AreEqual(routeInfoReturnDTO.Distance, result.Distance);
        }

        [Test]
        public void UpdateRouteInfo_Failure_RouteInfoRepositoryException()
        {
            // Arrange
            var routeInfoReturnDTO = new RouteInfoReturnDTO
            {
                RouteId = 999, // Assuming this ID does not exist in the database
                StartCity = "Updated City A",
                EndCity = "Updated City B",
                Distance = 150
            };

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(async () => await _adminRouteInfoService.UpdateRouteInfo(routeInfoReturnDTO));
        }

        [Test]
        public void UpdateRouteInfo_Failure_AdminRouteInfoServiceException()
        {
            // Arrange
            var routeInfoReturnDTO = new RouteInfoReturnDTO
            {
                RouteId = 1,
                StartCity = "Updated City A",
                EndCity = "Updated City B",
                Distance = 150
            };
            var repositoryMock = new Mock<IRepository<int, RouteInfo>>();
            repositoryMock.Setup(repo => repo.Update(It.IsAny<RouteInfo>())).ThrowsAsync(new Exception("Test exception"));
            var adminRouteInfoService = new AdminRouteInfoService(repositoryMock.Object, _scheduleRepository, Mock.Of<ILogger<AdminRouteInfoService>>());

            // Act & Assert
            Assert.ThrowsAsync<AdminRouteInfoServiceException>(async () => await adminRouteInfoService.UpdateRouteInfo(routeInfoReturnDTO));
        }


    }
}
