using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
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
    public class UserRepositoryTests
    {
        private FlightBookingContext _context;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            
            var userLoggerMock = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, userLoggerMock.Object);

        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };

            // Act
            var result = await _userRepository.Add(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [Test]
        public void Add_Failure_DuplicateUserException_Email()
        {
            // Arrange
            var user = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            _userRepository.Add(user).Wait();
            var duplicateUser = new User
            {
                Name = "Jane Doe",
                Email = "john@example.com", // Same email
                Phone = "0987654321",
                Role = "Customer"
            };

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(() => _userRepository.Add(duplicateUser));
        }

        [Test]
        public void Add_Failure_DuplicateUserException_Phone()
        {
            // Arrange
            var user = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            _userRepository.Add(user).Wait();

            var duplicateUser = new User
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Phone = "1234567890", // Same phone number
                Role = "Customer"
            };

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(() => _userRepository.Add(duplicateUser));
        }

        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidUser = new User();

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(() => _userRepository.Add(invalidUser));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            var addedUser = await _userRepository.Add(newUser);

            // Act
            var result = await _userRepository.GetByKey(addedUser.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedUser.UserId, result.UserId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(() => _userRepository.GetByKey(nonExistentUserId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            var addedUser = await _userRepository.Add(newUser);

            // Modify some data in the user
            addedUser.Name = "Updated John Doe";

            // Act
            var result = await _userRepository.Update(addedUser);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated John Doe", result.Name);
        }

        [Test]
        public async Task Update_Failure_UserNotFound()
        {
            // Arrange
            var nonExistentUser = new User
            {
                UserId = 999,
                Name = "Non-existent User",
                Email = "nonexistent@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(async () => await _userRepository.Update(nonExistentUser));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            var addedUser = await _userRepository.Add(newUser);

            // Act
            var result = await _userRepository.DeleteByKey(addedUser.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedUser.UserId, result.UserId);
        }

        [Test]
        public void DeleteByKey_Failure_UserNotFound()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(async () => await _userRepository.DeleteByKey(nonExistentUserId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newUser1 = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            };
            var newUser2 = new User
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Phone = "0987654321",
                Role = "Customer"
            };
            await _userRepository.Add(newUser1);
            await _userRepository.Add(newUser2);

            // Act
            var result = await _userRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_Failure_NoUsersPresent()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<UserServiceException>(async () => await _userRepository.GetAll());
        }
    }
}
