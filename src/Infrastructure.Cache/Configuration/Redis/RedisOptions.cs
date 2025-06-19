using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Cache.Configuration.Redis
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
