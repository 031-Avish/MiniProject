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
    public class UserDetailRepositoryTests
    {
        private FlightBookingContext _context;
        private UserDetailRepository _userDetailRepository;
        private UserRepository _userRepository;
        private User _user;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            _userDetailRepository = new UserDetailRepository(_context);
            _userRepository = new UserRepository(_context);

            // Initialize User
            _user = await _userRepository.Add(new User
            {
                Name = "John",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            });
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newUserDetail = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };

            // Act
            var result = await _userDetailRepository.Add(newUserDetail);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("john@example.com", result.Email);
        }

        [Test]
        public void Add_Failure_DuplicateUserException()
        {
            // Arrange
            var userDetail = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };
            _userDetailRepository.Add(userDetail).Wait();

            var duplicateUserDetail = new UserDetail
            {
                UserId = _user.UserId + 1,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(() => _userDetailRepository.Add(duplicateUserDetail));
        }

        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidUserDetail = new UserDetail();

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(() => _userDetailRepository.Add(invalidUserDetail));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newUserDetail = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };
            await _userDetailRepository.Add(newUserDetail);

            // Act
            var result = await _userDetailRepository.GetByKey(_user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(_user.UserId, result.UserId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentUserId = 999; // Assuming invalid user ID

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(() => _userDetailRepository.GetByKey(nonExistentUserId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newUserDetail = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };
            await _userDetailRepository.Add(newUserDetail);

            // Modify some data in the user detail
            newUserDetail.Email = "updated@example.com";

            // Act
            var result = await _userDetailRepository.Update(newUserDetail);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("updated@example.com", result.Email);
        }

        [Test]
        public async Task Update_Failure_UserDetailNotFound()
        {
            // Arrange
            var nonExistentUserDetail = new UserDetail
            {
                UserId = 999, // Assuming invalid user ID
                Email = "nonexistent@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(async () => await _userDetailRepository.Update(nonExistentUserDetail));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newUserDetail = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };
            await _userDetailRepository.Add(newUserDetail);

            // Act
            var result = await _userDetailRepository.DeleteByKey(_user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(_user.UserId, result.UserId);
        }

        [Test]
        public void DeleteByKey_Failure_UserDetailNotFound()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(async () => await _userDetailRepository.DeleteByKey(nonExistentUserId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newUserDetail1 = new UserDetail
            {
                UserId = _user.UserId,
                Email = "john@example.com",
                Password = new byte[] { 1, 2, 3 },
                PasswordHashKey = new byte[] { 4, 5, 6 }
            };
            var newUserDetail2 = new UserDetail
            {
                UserId = _user.UserId + 1,
                Email = "jane@example.com",
                Password = new byte[] { 7, 8, 9 },
                PasswordHashKey = new byte[] { 10, 11, 12 }
            };
            await _userDetailRepository.Add(newUserDetail1);
            await _userDetailRepository.Add(newUserDetail2);

            // Act
            var result = await _userDetailRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_Failure_NoUserDetailsPresent()
        {
            // Arrange 

            // Act & Assert
            Assert.ThrowsAsync<UserDetailRepositoryException>(async () => await _userDetailRepository.GetAll());
        }
    }
}
