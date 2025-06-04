namespace Infrastructure.Uploads.Configuration
{
    public sealed class CloudinaryOptions
    {
        public const string Cloudinary = "Cloudinary";

        public required string Url { get; set; }
    }
}
