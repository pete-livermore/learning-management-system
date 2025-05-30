using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Shared.Configuration.Redis
{
    public sealed class RedisOptions
    {
        public const string Redis = "Redis";

        [Required]
        public required string Configuration { get; set; }

        [Required]
        public required string InstanceName { get; set; }
    }
}
