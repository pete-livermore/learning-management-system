using Domain.Enums;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; init; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserRole Role { get; set; }
    }
}
