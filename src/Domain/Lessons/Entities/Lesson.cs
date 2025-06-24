using Domain.Common;

namespace Domain.Lessons.Entities;

public class Lesson : BaseEntity
{
    public int Id { get; private set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public ICollection<LessonSection> Sections = [];
    public required Guid OwnerId { get; init; }
}
