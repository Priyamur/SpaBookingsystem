using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Backend.Controllers.Tests
{
    [TestFixture]
    public class BackendControllerTests
    {
        private BackendController _controller;
        private AppDbContext _context;

        
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "test_db")
            .Options;
            _context = new AppDbContext(options);

            var hostingEnvironmentMock = new Mock<IWebHostEnvironment>();
            var configurationMock = new Mock<IConfiguration>();

            _controller = new BackendController(_context, hostingEnvironmentMock.Object, configurationMock.Object);
        }


       
        [Test]
        public async Task GetById_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetImage_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = _controller.GetImage(999);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
       
       
       
        [Test]
        public async Task DeleteServiceDetails_ValidId_ReturnsNoContent()
        {
            // Arrange
            var service = new Service
            {
                ServiceId = 1,
                ServiceName = "Test Service",
                ServiceDescription = "Test Description",
                ServiceCost = "10" // Set any required properties here
            };
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteServiceDetails(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
