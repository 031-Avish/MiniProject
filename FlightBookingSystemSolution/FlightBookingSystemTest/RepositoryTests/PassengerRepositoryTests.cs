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
    public class PassengerRepositoryTests
    {
        private FlightBookingContext _context;
        private PassengerRepository _passengerRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            _passengerRepository = new PassengerRepository(_context);
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newPassenger = new Passenger
            {
                Name = "John Doe",
                Age = 30,
                Gender = "Male"
            };

            // Act
            var result = await _passengerRepository.Add(newPassenger);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.PassengerId);
        }

        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidPassenger = new Passenger(); // Missing required fields

            // Act & Assert
            Assert.ThrowsAsync<PassengerRepositoryException>(() => _passengerRepository.Add(invalidPassenger));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newPassenger = new Passenger
            {
                Name = "John Doe",
                Age = 30,
                Gender = "Male"
            };
            var addedPassenger = await _passengerRepository.Add(newPassenger);

            // Act
            var result = await _passengerRepository.GetByKey(addedPassenger.PassengerId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedPassenger.PassengerId, result.PassengerId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentPassengerId = 999; // Assuming invalid passenger ID

            // Act & Assert
            Assert.ThrowsAsync<PassengerRepositoryException>(() => _passengerRepository.GetByKey(nonExistentPassengerId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newPassenger = new Passenger
            {
                Name = "John Doe",
                Age = 30,
                Gender = "Male"
            };
            var addedPassenger = await _passengerRepository.Add(newPassenger);

            // Modify some data in the passenger
            addedPassenger.Name = "Updated John Doe";

            // Act
            var result = await _passengerRepository.Update(addedPassenger);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated John Doe", result.Name);
        }

        [Test]
        public void Update_Failure_PassengerNotFound()
        {
            // Arrange
            var nonExistentPassenger = new Passenger
            {
                PassengerId = 999, // Assuming invalid passenger ID
                Name = "Non-Existent Passenger",
                Age = 30,
                Gender = "Male"
            };

            // Act & Assert
            Assert.ThrowsAsync<PassengerRepositoryException>(() => _passengerRepository.Update(nonExistentPassenger));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newPassenger = new Passenger
            {
                Name = "John Doe",
                Age = 30,
                Gender = "Male"
            };
            var addedPassenger = await _passengerRepository.Add(newPassenger);

            // Act
            var result = await _passengerRepository.DeleteByKey(addedPassenger.PassengerId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedPassenger.PassengerId, result.PassengerId);
        }

        [Test]
        public void DeleteByKey_Failure_PassengerNotFound()
        {
            // Arrange
            var nonExistentPassengerId = 999; // Assuming invalid passenger ID

            // Act & Assert
            Assert.ThrowsAsync<PassengerRepositoryException>(() => _passengerRepository.DeleteByKey(nonExistentPassengerId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newPassenger1 = new Passenger
            {
                Name = "John Doe",
                Age = 30,
                Gender = "Male"
            };
            var newPassenger2 = new Passenger
            {
                Name = "Jane Doe",
                Age = 28,
                Gender = "Female"
            };
            await _passengerRepository.Add(newPassenger1);
            await _passengerRepository.Add(newPassenger2);

            // Act
            var result = await _passengerRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count()); // Assuming 2 passengers were added
        }

        [Test]
        public void GetAll_Failure_NoPassengersPresent()
        {
            // Arrange
            // Ensure no passengers are added

            // Act & Assert
            Assert.ThrowsAsync<PassengerRepositoryException>(() => _passengerRepository.GetAll());
        }
    }
}
