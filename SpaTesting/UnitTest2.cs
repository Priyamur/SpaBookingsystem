using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Backend.Controllers;
using Backend.Models;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Backend.Data;
using MySqlX.XDevAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Backend.Tests
{
    [TestFixture]
    public class AppointmentsControllerTests
    {
        private AppointmentsController _controller;
        private Mock<AppDbContext> _contextMock;
        private List<Appointment> _appointments;
        private Mock<DbSet<Appointment>> _appointmentsMockSet;

        [SetUp]
        public void Setup()
        {
            _appointments = new List<Appointment>
{
new Appointment { AppointmentId = 1, Name = "Appointment 1" },
new Appointment { AppointmentId = 2, Name = "Appointment 2" },
new Appointment { AppointmentId = 3, Name = "Appointment 3" }
};

            _appointmentsMockSet = new Mock<DbSet<Appointment>>();
            _appointmentsMockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(_appointments.AsQueryable().Provider);
            _appointmentsMockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(_appointments.AsQueryable().Expression);
            _appointmentsMockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(_appointments.AsQueryable().ElementType);
            _appointmentsMockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(_appointments.AsQueryable().GetEnumerator());

            _contextMock = new Mock<AppDbContext>();
            _contextMock.Setup(c => c.Set<Appointment>()).Returns(_appointmentsMockSet.Object); // Set up DbSet directly

            _controller = new AppointmentsController(_contextMock.Object);
        }
        [Test]
        public async Task GetAppointments_ReturnsAllAppointments()
        {
            // Act
            var result = await _controller.GetAppointments();

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(_appointments.Count, result.Value.Count());
        }

        [Test]
        public async Task GetAppointment_WithValidId_ReturnsAppointment()
        {
            // Arrange
            var appointmentId = 1;

            // Act
            var result = await _controller.GetAppointment(appointmentId);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(appointmentId, result.Value.AppointmentId);
        }

        [Test]
        public async Task GetAppointment_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var appointmentId = 10;

            // Act
            var result = await _controller.GetAppointment(appointmentId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PutAppointment_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var appointmentId = 1;
            var appointment = new Appointment { AppointmentId = appointmentId, Name = "Updated Appointment" };

            // Act
            var result = await _controller.PutAppointment(appointmentId, appointment);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task PostAppointment_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var appointment = new Appointment
            {
                Name = "New Appointment",
                Date = "2024-03-29", // Provide a valid date here
                Time = "10:00", // Provide a valid time here
                PhoneNumber = 1234567890, // Provide a valid phone number here
                ClientAge = "30", // Provide a valid client age here
                ClientId = 1, // Provide a valid client ID here
                ServiceId = 1 // Provide a valid service ID here
            };

            // Act
            var result = await _controller.PostAppointment(
                appointment.Name,
                appointment.Date,
                appointment.Time,
                appointment.PhoneNumber,
                appointment.ClientAge,
                appointment.ClientId,
                appointment.ServiceId);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
        }
        [Test]
        public async Task DeleteAppointment_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var appointmentId = 1;

            // Act
            var result = await _controller.DeleteAppointment(appointmentId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}