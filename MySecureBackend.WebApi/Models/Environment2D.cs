namespace MySecureBackend.WebApi.Models
{
    public class Environment2D
    {
        public Guid EnvironmentId { get; set; }
        public string Name { get; set; }
        public string? OwnerUserId { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxHeight { get; set; }
    }
}
