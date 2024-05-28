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
    public class PaymentRepositoryTests
    {
        private FlightBookingContext _context;
        private PaymentRepository _paymentRepository;
        private BookingRepository _bookingRepository;
        private UserRepository _userRepository;
        private ScheduleRepository _scheduleRepository;
        private User _user;
        private Schedule _schedule;
        private Booking _booking;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            //_paymentRepository = new PaymentRepository(_context);
            //_bookingRepository = new BookingRepository(_context);
            //_userRepository = new UserRepository(_context);
            //_scheduleRepository = new ScheduleRepository(_context);
      
            var paymentLoggerMock = new Mock<ILogger<PaymentRepository>>();
            _paymentRepository = new PaymentRepository(_context, paymentLoggerMock.Object);
            var bookingLoggerMock = new Mock<ILogger<BookingRepository>>();
            _bookingRepository = new BookingRepository(_context, bookingLoggerMock.Object);
            var userLoggerMock = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, userLoggerMock.Object);
            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);



        }
        public async Task setup()
        {
            // Initialize User
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
                DepartureTime = DateTime.Now.AddHours(1),
                ReachingTime = DateTime.Now.AddHours(3),
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

            // Initialize Booking
            _booking = await _bookingRepository.Add(new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            });
        }
        [Test]
        public async Task Add_Success()
        {
            setup();
            // Arrange
            var newPayment = new Payment
            {
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };

            // Act
            var result = await _paymentRepository.Add(newPayment);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.PaymentId);
        }

        [Test]
        public async Task GetByKey_Success()
        {
            setup();
            // Arrange
            var newPayment = new Payment
            {
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };
            var addedPayment = await _paymentRepository.Add(newPayment);

            // Act
            var result = await _paymentRepository.GetByKey(addedPayment.PaymentId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedPayment.PaymentId, result.PaymentId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentPaymentId = 999; // Assuming invalid payment ID

            // Act & Assert
            Assert.ThrowsAsync<PaymentRepositoryException>(() => _paymentRepository.GetByKey(nonExistentPaymentId));
        }

        [Test]
        public async Task Update_Success()
        {
            setup();
            // Arrange
            var newPayment = new Payment
            {
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };
            var addedPayment = await _paymentRepository.Add(newPayment);

            // Modify some data in the payment
            addedPayment.PaymentStatus = "Pending";

            // Act
            var result = await _paymentRepository.Update(addedPayment);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Pending", result.PaymentStatus);
        }

        [Test]
        public void Update_Failure_PaymentNotFound()
        {
            setup();
            // Arrange
            var nonExistentPayment = new Payment
            {
                PaymentId = 999, // Assuming invalid payment ID
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };

            // Act & Assert
            Assert.ThrowsAsync<PaymentRepositoryException>(() => _paymentRepository.Update(nonExistentPayment));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            setup();
            // Arrange
            var newPayment = new Payment
            {
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };
            var addedPayment = await _paymentRepository.Add(newPayment);

            // Act
            var result = await _paymentRepository.DeleteByKey(addedPayment.PaymentId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedPayment.PaymentId, result.PaymentId);
        }

        [Test]
        public void DeleteByKey_Failure_PaymentNotFound()
        {

            // Arrange
            var nonExistentPaymentId = 999; // Assuming invalid payment ID

            // Act & Assert
            Assert.ThrowsAsync<PaymentRepositoryException>(() => _paymentRepository.DeleteByKey(nonExistentPaymentId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            setup();
            // Arrange
            var newPayment1 = new Payment
            {
                Amount = 200.0f,
                PaymentStatus = "Completed",
                BookingId = _booking.BookingId
            };
            var newPayment2 = new Payment
            {
                Amount = 300.0f,
                PaymentStatus = "Pending",
                BookingId = _booking.BookingId
            };
            await _paymentRepository.Add(newPayment1);
            await _paymentRepository.Add(newPayment2);

            // Act
            var result = await _paymentRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count()); // Assuming 2 payments were added
        }

        [Test]
        public void GetAll_Failure_NoPaymentsPresent()
        {
            // Arrange
            // Ensure no payments are added

            // Act & Assert
            Assert.ThrowsAsync<PaymentRepositoryException>(() => _paymentRepository.GetAll());
        }
    }
}
