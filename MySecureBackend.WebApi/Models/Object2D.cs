using System.ComponentModel.DataAnnotations;

namespace MySecureBackend.WebApi.Models
{
    public class Object2D
    {
        public Guid Id { get; set; }
        public required Guid EnvironmentId { get; set; }

        [Required]
        public required string PrefabId { get; set; }
        public required float PositionX { get; set; }
        public required float PositionY { get; set; }
        public required float ScaleX { get; set; }
        public required float ScaleY { get; set; }
        public required float RotationZ { get; set; }
        public required int SortingLayer { get; set; }
    }
}
