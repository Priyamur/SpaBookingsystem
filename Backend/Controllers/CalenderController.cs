using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class CalenderController:ControllerBase
    {
        private readonly AppDbContext _context;

        public CalenderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCalenders()
        {
            var currentDate =  DateOnly.FromDateTime(DateTime.Today);
            var availableCalenders = await _context.Calenders.Where(a => a.IsAvailable && a.NumberOfBookings < 2 && a.Date >= currentDate).ToListAsync();
            return Ok(availableCalenders);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookCalender(int calenderId)
        {
            var calender = await _context.Calenders.FindAsync(calenderId);

            if (calender == null)

            {
                return NotFound();
            }
            if (!calender.IsAvailable || calender.NumberOfBookings >= 2)

            {

                return BadRequest("Appointment is not available.");

            }
            calender.NumberOfBookings++;
            if (calender.NumberOfBookings >= 2)
            {
                calender.IsAvailable = false;
            }
            await _context.SaveChangesAsync();
            return Ok(calender);
        }

        }
    }
