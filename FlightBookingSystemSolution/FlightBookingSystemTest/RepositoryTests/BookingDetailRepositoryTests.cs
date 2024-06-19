using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace FlightBookingSystemTest.RepositoryTests
{
    public class BookingDetailRepositoryTests
    {
        private FlightBookingContext _context;
        private BookingDetailRepository _bookingDetailRepository;
        private UserRepository _userRepository;
        private ScheduleRepository _scheduleRepository;
        private BookingRepository _bookingRepository;
        private PassengerRepository _passengerRepository;
        private User _user;
        private Schedule _schedule;
        private Booking _booking;
        private Passenger _passenger;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);

            var loggerMock = new Mock<ILogger<BookingDetailRepository>>();
            _bookingDetailRepository = new BookingDetailRepository(_context, loggerMock.Object);

            var userLoggerMock = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, userLoggerMock.Object);

            var scheduleLoggerMock = new Mock<ILogger<ScheduleRepository>>();
            _scheduleRepository = new ScheduleRepository(_context, scheduleLoggerMock.Object);

            var bookingLoggerMock = new Mock<ILogger<BookingRepository>>();
            _bookingRepository = new BookingRepository(_context, bookingLoggerMock.Object);

            var passengerLoggerMock = new Mock<ILogger<PassengerRepository>>();
            _passengerRepository = new PassengerRepository(_context, passengerLoggerMock.Object);

            await SetupData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task SetupData()
        {
            _user = await _userRepository.Add(new User
            {
                Name = "John",
                Email = "john@example.com",
                Phone = "1234567890",
                Role = "Customer"
            });

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

            _booking = await _bookingRepository.Add(new Booking
            {
                BookingStatus = "Confirmed",
                BookingDate = DateTime.Now,
                PaymentStatus = "Pending",
                TotalPrice = 100.0f,
                UserId = _user.UserId,
                ScheduleId = _schedule.ScheduleId
            });

            _passenger = await _passengerRepository.Add(new Passenger
            {
                Age = 23,
                Name = "karn",
                Gender = "Male"
            });
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newBookingDetail = new BookingDetail
            {
                PassengerId =_passenger.PassengerId,
                BookingId = _booking.BookingId
            };

            // Act
            var result = await _bookingDetailRepository.Add(newBookingDetail);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(_passenger.PassengerId, result.PassengerId);
            Assert.AreEqual(_booking.BookingId, result.BookingId);
        }

        //[Test]
        //public void Add_Failure_Exception()
        //{
        //    // Arrange
        //    var invalidBookingDetail = new BookingDetail(); // Missing required fields

        //    // Act & Assert
        //    Assert.ThrowsAsync<BookingDetailRepositoryException>(() => _bookingDetailRepository.Add(invalidBookingDetail));
        //}

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newBookingDetail = new BookingDetail
            {
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };
            var addedBookingDetail = await _bookingDetailRepository.Add(newBookingDetail);

            // Act
            var result = await _bookingDetailRepository.GetByKey(addedBookingDetail.BookingDetailId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedBookingDetail.BookingDetailId, result.BookingDetailId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentBookingDetailId = 999;

            // Act & Assert
            Assert.ThrowsAsync<BookingDetailRepositoryException>(() => _bookingDetailRepository.GetByKey(nonExistentBookingDetailId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newBookingDetail = new BookingDetail
            {
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };
            var addedBookingDetail = await _bookingDetailRepository.Add(newBookingDetail);

            // Modify some data in the booking detail
            addedBookingDetail.PassengerId = _passenger.PassengerId;

            // Act
            var result = await _bookingDetailRepository.Update(addedBookingDetail);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(_passenger.PassengerId, result.PassengerId);
        }

        [Test]
        public async Task Update_Failure_BookingDetailNotFound()
        {
            // Arrange
            var nonExistentBookingDetail = new BookingDetail
            {
                BookingDetailId = 999,
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };

            // Act & Assert
            Assert.ThrowsAsync<BookingDetailRepositoryException>(async () => await _bookingDetailRepository.Update(nonExistentBookingDetail));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newBookingDetail = new BookingDetail
            {
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };
            var addedBookingDetail = await _bookingDetailRepository.Add(newBookingDetail);

            // Act
            var result = await _bookingDetailRepository.DeleteByKey(addedBookingDetail.BookingDetailId);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void DeleteByKey_Failure_BookingDetailNotFound()
        {
            // Arrange
            var nonExistentBookingDetailId = 999;

            // Act & Assert
            var exception = Assert.ThrowsAsync<BookingDetailRepositoryException>(async () => await _bookingDetailRepository.DeleteByKey(nonExistentBookingDetailId));
            Assert.NotNull(exception);
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newBookingDetail1 = new BookingDetail
            {
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };
            var newBookingDetail2 = new BookingDetail
            {
                PassengerId = _passenger.PassengerId,
                BookingId = _booking.BookingId
            };
            await _bookingDetailRepository.Add(newBookingDetail1);
            await _bookingDetailRepository.Add(newBookingDetail2);

            // Act
            var result = await _bookingDetailRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_Failure_NoBookingDetailsPresent()
        {
            // Act & Assert
            Assert.ThrowsAsync<BookingDetailRepositoryException>(async () => await _bookingDetailRepository.GetAll());
        }

        [Test]
        public async Task GetAllException()
        {
            // Simulate an exception during GetAll
            _context.BookingDetails = null; // Setting it to null will cause an exception

            // Act & Assert
            var exception = Assert.ThrowsAsync<BookingDetailRepositoryException>(async () =>
            {
                await _bookingDetailRepository.GetAll();
            });
        }

        [Test]
        public async Task GetByKeyException()
        {
            // Simulate an exception during GetByKey
            _context.BookingDetails = null; // Setting it to null will cause an exception

            // Act & Assert
            var exception = Assert.ThrowsAsync<BookingDetailRepositoryException>(async () =>
            {
                await _bookingDetailRepository.GetByKey(999);
            });
        }
    }
}
