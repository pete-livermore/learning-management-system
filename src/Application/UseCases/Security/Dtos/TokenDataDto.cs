namespace Application.UseCases.Security.Dtos
{
    public record TokenDataDto
    {
        public required Guid UserId { get; set; }
        public required string Email { get; set; }
        public required List<string> Roles { get; set; }
    }
}
