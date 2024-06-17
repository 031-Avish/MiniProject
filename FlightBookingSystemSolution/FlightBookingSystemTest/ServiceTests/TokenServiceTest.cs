using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FlightBookingSystemTest.ServiceTests
{
    public class TokenServiceTest
    {
        private FlightBookingContext _context;
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new FlightBookingContext(options);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);
        }

        [Test]
        public void CreateTokenPassTest()
        {
            // Arrange 
            var user = new User
            {
                UserId = 101,
                Role = "User",
                Email = "avi@gmail.com",
                Name = "Avi",
                Phone = "9876543210"
            };

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
        }
    }
}
