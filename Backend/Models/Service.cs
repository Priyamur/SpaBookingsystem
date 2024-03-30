using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        [Required]
        public string? ServiceName { get; set; }
        [Required]
        public string? ServiceDescription { get; set; }
        [Required]
        public string? ServiceCost { get; set; }
        
        [NotMapped]
        public IFormFile? ServiceImage { get; set; }

        public string? UniqueFileName { get; set; }


    }
}
