using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            var clientid = appointment.ClientId;
            var clients = await _context.Clients.FindAsync(clientid);
            var serviceid = appointment.ServiceId;
            var services = await _context.Services.FindAsync(serviceid);

            appointment.Client= clients;
            appointment.Service= services;

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(string name, string date, string time, long phoneNumber, string clientAge, int clientId, int serviceId)
        {
            var client = await  _context.Clients.FindAsync(clientId);
            var service = await _context.Services.FindAsync(serviceId);
            var appointmentDate = DateOnly.Parse(date);
            var calender = await _context.Calenders.FirstOrDefaultAsync(c => c.Date == appointmentDate);
            if(client == null || service == null)
            {
                return NotFound("Data not found");
            }
            var appointment = new Appointment
            {
                Name = name,
                Date = appointmentDate,
                Time = time,
                PhoneNumber = phoneNumber,
                CalenderId = calender.Id,
                ClientAge = clientAge,
                ServiceId = serviceId,
                ClientId = clientId
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            
            var appointment = await _context.Appointments.FindAsync(id);
            var calender = await _context.Calenders.FirstOrDefaultAsync(c => c.Id == appointment.CalenderId);
            calender.NumberOfBookings -= 1;
            calender.IsAvailable = true;
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
