using Application.Common.Dtos;

namespace Application.UseCases.Uploads.Dtos
{
    public record FileDto
    {
        public int Id { get; init; }
        public required string Url { get; init; }
        public required string Mime { get; set; }
        public required string Ext { get; set; }
        public required string ProviderId { get; set; }
        public required EntityOwnerDto Owner { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
