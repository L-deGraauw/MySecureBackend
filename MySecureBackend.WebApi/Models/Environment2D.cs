using System.ComponentModel.DataAnnotations;

namespace MySecureBackend.WebApi.Models
{
    public class Environment2D
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string? OwnerUserId { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxHeight { get; set; }
    }
}
