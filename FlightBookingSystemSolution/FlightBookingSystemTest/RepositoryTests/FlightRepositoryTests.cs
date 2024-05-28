using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBookingSystemTest.RepositoryTests
{
    public class FlightRepositoryTests
    {
        private FlightBookingContext _context;
        private FlightRepository _flightRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            _flightRepository = new FlightRepository(_context);
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newFlight = new Flight
            {
                Name = "Test Airline",
                TotalSeats = 150
            };

            // Act
            var result = await _flightRepository.Add(newFlight);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.FlightId);
        }

        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidFlight = new Flight(); // Missing required fields

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _flightRepository.Add(invalidFlight));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newFlight = new Flight
            {
                Name = "Test Airline",
                TotalSeats = 150
            };
            var addedFlight = await _flightRepository.Add(newFlight);

            // Act
            var result = await _flightRepository.GetByKey(addedFlight.FlightId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedFlight.FlightId, result.FlightId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentFlightId = 999; // Assuming invalid flight ID

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _flightRepository.GetByKey(nonExistentFlightId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newFlight = new Flight
            {
                Name = "Test Airline",
                TotalSeats = 150
            };
            var addedFlight = await _flightRepository.Add(newFlight);

            // Modify some data in the flight
            addedFlight.Name = "Updated Airline";

            // Act
            var result = await _flightRepository.Update(addedFlight);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated Airline", result.Name);
        }

        [Test]
        public void Update_Failure_FlightNotFound()
        {
            // Arrange
            var nonExistentFlight = new Flight
            {
                FlightId = 999, // Assuming invalid flight ID
                Name = "Non-Existent Airline",
                TotalSeats = 150
            };

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _flightRepository.Update(nonExistentFlight));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newFlight = new Flight
            {
                Name = "Test Airline",
                TotalSeats = 150
            };
            var addedFlight = await _flightRepository.Add(newFlight);

            // Act
            var result = await _flightRepository.DeleteByKey(addedFlight.FlightId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedFlight.FlightId, result.FlightId);
        }

        [Test]
        public void DeleteByKey_Failure_FlightNotFound()
        {
            // Arrange
            var nonExistentFlightId = 999; // Assuming invalid flight ID

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _flightRepository.DeleteByKey(nonExistentFlightId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newFlight1 = new Flight
            {
                Name = "Test Airline 1",
                TotalSeats = 150
            };
            var newFlight2 = new Flight
            {
                Name = "Test Airline 2",
                TotalSeats = 200
            };
            await _flightRepository.Add(newFlight1);
            await _flightRepository.Add(newFlight2);

            // Act
            var result = await _flightRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            int i = 0;
            foreach (var flight in result)
            {
                i++;
                Assert.AreEqual(i, flight.FlightId);
            }
            Assert.AreEqual(2, result.Count()); // Assuming 2 flights were added
        }

        [Test]
        public void GetAll_Failure_NoFlightsPresent()
        {
            // Arrange
            // Ensure no flights are added

            // Act & Assert
            Assert.ThrowsAsync<FlightRepositoryException>(() => _flightRepository.GetAll());
        }
    }
}
