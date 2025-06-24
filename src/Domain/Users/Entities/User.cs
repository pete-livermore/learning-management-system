using Domain.Common;
using Domain.Lessons.Entities;
using Domain.Users.Enums;

namespace Domain.Users.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; init; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required UserRole Role { get; set; }
        public required Guid ApplicationUserId { get; set; }
        public ICollection<LessonSectionProgress> LessonSectionProgresses { get; set; } = [];
    }
}
