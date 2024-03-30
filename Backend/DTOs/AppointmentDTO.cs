using Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class AppointmentDTO
    {
        [Key]
        public int AppointmentId { get; set; }

        public string? Name { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public long PhoneNumber { get; set; }

        public string? Allergyspecification { get; set; }

        public string? ClientAge { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
    }
}
