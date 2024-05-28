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
    public class RouteInfoRepositoryTests
    {
        private FlightBookingContext _context;
        private RouteInfoRepository _routeInfoRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FlightBookingContext>()
                .UseInMemoryDatabase(databaseName: "dummyDB")
                .Options;
            _context = new FlightBookingContext(options);
            _routeInfoRepository = new RouteInfoRepository(_context);
        }

        [Test]
        public async Task Add_Success()
        {
            // Arrange
            var newRouteInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };

            // Act
            var result = await _routeInfoRepository.Add(newRouteInfo);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.RouteId);
        }

        [Test]
        public void Add_Failure_Exception()
        {
            // Arrange
            var invalidRouteInfo = new RouteInfo();

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(() => _routeInfoRepository.Add(invalidRouteInfo));
        }

        [Test]
        public async Task GetByKey_Success()
        {
            // Arrange
            var newRouteInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var addedRouteInfo = await _routeInfoRepository.Add(newRouteInfo);

            // Act
            var result = await _routeInfoRepository.GetByKey(addedRouteInfo.RouteId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedRouteInfo.RouteId, result.RouteId);
        }

        [Test]
        public void GetByKey_Failure_NotFoundException()
        {
            // Arrange
            var nonExistentRouteInfoId = 999;

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(() => _routeInfoRepository.GetByKey(nonExistentRouteInfoId));
        }

        [Test]
        public async Task Update_Success()
        {
            // Arrange
            var newRouteInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var addedRouteInfo = await _routeInfoRepository.Add(newRouteInfo);

            // Modify some data in the route info
            addedRouteInfo.StartCity = "Updated City";

            // Act
            var result = await _routeInfoRepository.Update(addedRouteInfo);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Updated City", result.StartCity);
        }

        [Test]
        public void Update_Failure_RouteInfoNotFound()
        {
            // Arrange
            var nonExistentRouteInfo = new RouteInfo
            {
                RouteId = 999,
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(() => _routeInfoRepository.Update(nonExistentRouteInfo));
        }

        [Test]
        public async Task DeleteByKey_Success()
        {
            // Arrange
            var newRouteInfo = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var addedRouteInfo = await _routeInfoRepository.Add(newRouteInfo);

            // Act
            var result = await _routeInfoRepository.DeleteByKey(addedRouteInfo.RouteId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(addedRouteInfo.RouteId, result.RouteId);
        }

        [Test]
        public void DeleteByKey_Failure_RouteInfoNotFound()
        {
            // Arrange
            var nonExistentRouteInfoId = 999; // Assuming invalid route info ID

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(() => _routeInfoRepository.DeleteByKey(nonExistentRouteInfoId));
        }

        [Test]
        public async Task GetAll_Success()
        {
            // Arrange
            var newRouteInfo1 = new RouteInfo
            {
                StartCity = "City A",
                EndCity = "City B",
                Distance = 100
            };
            var newRouteInfo2 = new RouteInfo
            {
                StartCity = "City C",
                EndCity = "City D",
                Distance = 200
            };
            await _routeInfoRepository.Add(newRouteInfo1);
            await _routeInfoRepository.Add(newRouteInfo2);

            // Act
            var result = await _routeInfoRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_Failure_NoRouteInfosPresent()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<RouteInfoRepositoryException>(() => _routeInfoRepository.GetAll());
        }
    }
}
