using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBookingSystemTest.RepositoryTests
{
    public class ScheduleRepositoryTests
    {
        private FlightBookingContext _context;
        private ScheduleRepository _scheduleRepository;
        private Flight _flight;
        private RouteInfo _routeInfo;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);
            // Adding Flight
            _flight = new Flight
            {
                Name = "ABC Airlines",
                TotalSeats = 150
            };
            _context.Flights.Add(_flight);
            await _context.SaveChangesAsync();

            // Adding RouteInfo
            _routeInfo = new RouteInfo
            {
                StartCity = "New York",
                EndCity = "Los Angeles",
                Distance = 2500.0f
            };
            _context.RouteInfos.Add(_routeInfo);
            await _context.SaveChangesAsync();

        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newSchedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };

            // Act
            var result = await _scheduleRepository.Add(newSchedule);


            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.ScheduleId);
        }

        [Test]
        public async Task Add_Failure_Exception()
        {
            // Arrange
            var invalidSchedule = new Schedule();
            var schedule = await _scheduleRepository.Add(invalidSchedule);
            // Act & Assert
            var result = await _scheduleRepository.GetByKey(schedule.ScheduleId);

            //Assert.ThrowsAsync<ScheduleRepositoryException>();
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newSchedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };
            var addedSchedule = await _scheduleRepository.Add(newSchedule);

            // Act
            var result = await _scheduleRepository.GetByKey(addedSchedule.ScheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedSchedule.ScheduleId, result.ScheduleId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentScheduleId = 999;

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(() => _scheduleRepository.GetByKey(nonExistentScheduleId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newSchedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };
            var addedSchedule = await _scheduleRepository.Add(newSchedule);

            // Modify some data in the schedule
            addedSchedule.AvailableSeat = 90;

            // Act
            var result = await _scheduleRepository.Update(addedSchedule);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(90, result.AvailableSeat);
        }

        [Test]
        public void Update_Failure_ScheduleNotFound()
        {
            // Arrange
            var nonExistentSchedule = new Schedule
            {
                ScheduleId = 999,
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(() => _scheduleRepository.Update(nonExistentSchedule));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newSchedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };
            var addedSchedule = await _scheduleRepository.Add(newSchedule);

            // Act
            var result = await _scheduleRepository.DeleteByKey(addedSchedule.ScheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedSchedule.ScheduleId, result.ScheduleId);
        }

        [Test]
        public void DeleteByKey_Failure_ScheduleNotFound()
        {
            // Arrange
            var nonExistentScheduleId = 999;

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(() => _scheduleRepository.DeleteByKey(nonExistentScheduleId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newSchedule1 = new Schedule
            {
                DepartureTime = DateTime.Now,
                ReachingTime = DateTime.Now.AddHours(2),
                AvailableSeat = 100,
                Price = 150.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };
            var newSchedule2 = new Schedule
            {
                DepartureTime = DateTime.Now.AddDays(1),
                ReachingTime = DateTime.Now.AddDays(1).AddHours(2),
                AvailableSeat = 80,
                Price = 200.0f,
                RouteId = _routeInfo.RouteId,
                FlightId = _flight.FlightId
            };
            await _scheduleRepository.Add(newSchedule1);
            await _scheduleRepository.Add(newSchedule2);

            // Act
            var result = await _scheduleRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_Failure_NoSchedulesPresent()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<ScheduleRepositoryException>(() => _scheduleRepository.GetAll());
        }
    }
}


