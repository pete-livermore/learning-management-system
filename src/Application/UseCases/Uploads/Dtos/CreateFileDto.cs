namespace Application.UseCases.Uploads.Dtos
{
    public record CreateFileDto
    {
        public required string Url { get; init; }
        public required string Mime { get; set; }
        public required string Ext { get; set; }
        public required string ProviderId { get; set; }
        public required string ResourceType { get; set; }
        public required int OwnerId { get; set; }
    }
}
