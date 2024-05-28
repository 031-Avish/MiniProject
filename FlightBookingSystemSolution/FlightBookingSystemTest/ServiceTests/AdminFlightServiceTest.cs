using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Repositories;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlightBookingSystemAPI.Tests.Services
{
    public class AdminFlightServiceTests
    {
        private FlightBookingContext _context;
        private IRepository<int, Flight> _flightRepository;
        private IRepository<int, Schedule> _scheduleRepository;
        private AdminFlightService _adminFlightService;
        
        
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
            
            _adminFlightService = new AdminFlightService(_flightRepository, _scheduleRepository);
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
            //Assert.AreEqual(1, result.FlightId); 
            Assert.AreEqual(flightDTO.Name, result.Name);
            Assert.AreEqual(flightDTO.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void AddFlight_Failure_FlightRepositoryException()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight"
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _adminFlightService.AddFlight(flightDTO));
        }

        [Test]
        public void AddFlight_Failure_Exception()
        {
            // Arrange
            var flightDTO = new FlightDTO
            {
                Name = "Test Flight",
                TotalSeats = 150
            };

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(() => _adminFlightService.AddFlight(flightDTO));
        }

        [Test]
        public async Task DeleteFlight_Success()
        {
            // Arrange
            var flight = new Flight
            {
                FlightId = 1,
                Name = "Test Flight",
                TotalSeats = 150
            };

            // Add flight to context
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminFlightService.DeleteFlight(flight.FlightId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flight.FlightId, result.FlightId);
            Assert.AreEqual(flight.Name, result.Name);
            Assert.AreEqual(flight.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void DeleteFlight_Failure_FlightRepositoryException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _adminFlightService.DeleteFlight(1));
        }

        [Test]
        public void DeleteFlight_Failure_Exception()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(() => _adminFlightService.DeleteFlight(1));
        }

        [Test]
        public async Task GetAllFlight_Success()
        {
            // Arrange
            var flights = new List<Flight>
            {
                new Flight { FlightId = 1, Name = "Flight 1", TotalSeats = 100 },
                new Flight { FlightId = 2, Name = "Flight 2", TotalSeats = 150 }
            };

            // Add flights to context
            _context.Flights.AddRange(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminFlightService.GetAllFlight();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetAllFlight_Failure_UserRepositoryException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<UserRepositoryException>(() => _adminFlightService.GetAllFlight());
        }

        [Test]
        public void GetAllFlight_Failure_Exception()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(() => _adminFlightService.GetAllFlight());
        }

        [Test]
        public async Task GetFlight_Success()
        {
            // Arrange
            var flight = new Flight
            {
                FlightId = 1,
                Name = "Test Flight",
                TotalSeats = 150
            };

            // Add flight to context
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
        public void GetFlight_Failure_UserRepositoryException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<UserRepositoryException>(() => _adminFlightService.GetFlight(1));
        }

        [Test]
        public void GetFlight_Failure_Exception()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(() => _adminFlightService.GetFlight(1));
        }

        [Test]
        public async Task UpdateFlight_Success()
        {
            // Arrange
            var flightReturnDTO = new FlightReturnDTO
            {
                FlightId = 1,
                Name = "Updated Flight",
                TotalSeats = 200
            };

            var flight = new Flight
            {
                FlightId = 1,
                Name = "Updated Flight",
                TotalSeats = 200
            };

            // Add flight to context
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _adminFlightService.UpdateFlight(flightReturnDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(flight.FlightId, result.FlightId);
            Assert.AreEqual(flight.Name, result.Name);
            Assert.AreEqual(flight.TotalSeats, result.TotalSeats);
        }

        [Test]
        public void UpdateFlight_Failure_UserRepositoryException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<UserRepositoryException>(() => _adminFlightService.UpdateFlight(new FlightReturnDTO()));
        }

        [Test]
        public void UpdateFlight_Failure_Exception()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<AdminFlightServiceException>(() => _adminFlightService.UpdateFlight(new FlightReturnDTO()));
        }
    }
}

//[TearDown]
//        public void TearDown()
//        {
//            _context.Database.EnsureDeleted(); // Ensures that the in-memory database is deleted after each test
//            _context.Dispose(); // Dispose the context to release resources
//        }
//    }

