using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Calender
    {
        [Key]
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public string? Time { get; set; }

        public bool IsAvailable { get; set; }

        public int NumberOfBookings { get; set; }
    }
}
