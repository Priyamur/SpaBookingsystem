using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace Backend.DTOs
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }

        public string? ServiceDescription { get; set; }

        public string? ServiceCost { get; set; }

        [NotMapped]
        public IFormFile? ServiceImage { get; set; } // Image file for the service 

        public string? UniqueFileName { get; set; }

    }
}
