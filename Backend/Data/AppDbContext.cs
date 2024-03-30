using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public  DbSet<Appointment> Appointments { get; set; } 

        public DbSet<Service> Services { get; set; }

        public DbSet<Client> Clients { get; set; } 
        
        public DbSet<Calender> Calenders { get; set; }
    }
}
