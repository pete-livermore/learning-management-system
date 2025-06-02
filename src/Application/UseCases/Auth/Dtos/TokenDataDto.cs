namespace Application.Dtos.Auth
{
    public record TokenDataDto
    {
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
