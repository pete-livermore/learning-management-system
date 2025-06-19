namespace Application.Uploads.Dtos
{
    public class UploadProviderMetadata
    {
        public required string Id { get; set; }
    }

    public record UploadedFileDto
    {
        public required string Url { get; set; }

        public required string ResourceType { get; set; }
        public required UploadProviderMetadata ProviderMetadata { get; set; }
    }
}
