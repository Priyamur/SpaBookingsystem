using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class JwtSettings
    {
        [Required]
        public string? Secret { get; set; }

        [Required]
        public string? Issuer { get; set; }

        [Required]
        public string? Audience { get; set; }

        public string? Key { get; set; }
    }
}