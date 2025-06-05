using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Uploads.Configuration
{
    public sealed class CloudinaryOptions
    {
        public const string Cloudinary = "Cloudinary";

        [Required]
        public required string Url { get; set; }
    }
}
