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
    public class BookingRepositoryTests
    {
        private FlightBookingContext _context;
        private BookingRepository _bookingRepository;
        private UserRepository _userRepository;
        private ScheduleRepository _scheduleRepository;
        private User _user;
        private Schedule _schedule;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            _bookingRepository = new BookingRepository(_context);
            _userRepository = new UserRepository(_context);
            _scheduleRepository = new ScheduleRepository(_context);

            // Initialize User

        }
        public async Task setup()
        {
            _user = await _userRepository.Add(new User
            {
                Name = "John",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            });

            // Initialize Schedule
            _schedule = await _scheduleRepository.Add(new Schedule
            {
                DepartureTime = DateTime.Now.AddHours(1), // Example departure time
                ReachingTime = DateTime.Now.AddHours(3), // Example reaching time
                AvailableSeat = 100,
                Price = 150.0f,
                RouteInfo = new RouteInfo
                {
                    StartCity = "New York",
                    EndCity = "Los Angeles",
                    Distance = 2500.0f
                },
                FlightInfo = new Flight
                {
                    Name = "ABC Airlines",
                    TotalSeats = 150
                }
            });
        }
        [Test]
        public async Task Add_Success()
        {
            setup();
            // Arrange
            var newBooking = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };

            // Act
            var result = await _bookingRepository.Add(newBooking);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(100.0f, result.TotalPrice);

        }
        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidBooking = new Booking(); // Missing required fields

            // Act & Assert
            Assert.ThrowsAsync<BookingRepositoryException>(() => _bookingRepository.Add(invalidBooking));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newBooking = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };
            var addedBooking = await _bookingRepository.Add(newBooking);

            // Act
            var result = await _bookingRepository.GetByKey(2);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.BookingId);
        }
        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentBookingId = 999; // Assuming invalid booking ID

            // Act & Assert
            Assert.ThrowsAsync<BookingRepositoryException>(() => _bookingRepository.GetByKey(nonExistentBookingId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newBooking = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };
            var addedBooking = await _bookingRepository.Add(newBooking);

            // Modify some data in the booking
            addedBooking.BookingStatus = "Updated";

            // Act
            var result = await _bookingRepository.Update(addedBooking);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated", result.BookingStatus);

        }

        [Test]
        public async Task Update_Failure_BookingNotFound()
        {
            // Arrange
            var nonExistentBooking = new Booking
            {
                BookingId = 999, // Assuming invalid booking ID
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };

            // Act & Assert
            Assert.ThrowsAsync<BookingRepositoryException>(async () => await _bookingRepository.Update(nonExistentBooking));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            setup();
            // Arrange
            var newBooking = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };
            var addedBooking = await _bookingRepository.Add(newBooking);

            // Act
            var result = await _bookingRepository.DeleteByKey(addedBooking.BookingId);

            // Assert
            Assert.NotNull(result);

        }
        [Test]
        public async Task DeleteByKey_Failure_BookingNotFound()
        {
            // Arrange
            var nonExistentBookingId = 999; // Assuming invalid booking ID

            // Act & Assert
            Assert.ThrowsAsync<BookingRepositoryException>(async () => await _bookingRepository.DeleteByKey(nonExistentBookingId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newBooking1 = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };
            var newBooking2 = new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now.AddDays(-1),
                PaymentStatus = "Pending",
                TotalPrice = 200.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            };
            await _bookingRepository.Add(newBooking1);
            await _bookingRepository.Add(newBooking2);

            // Act
            var result = await _bookingRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count()); // Assuming 2 bookings were added
        }
        [Test]
        public void GetAll_Failure_NoBookingsPresent()
        {
            // Arrange
            // Ensure no bookings are added

            // Act & Assert
            Assert.ThrowsAsync<BookingRepositoryException>(async () => await _bookingRepository.GetAll());
        }
    }

}
