using Domain.Common;

namespace Domain.Lessons.Entities;

public class LessonSectionProgress : BaseEntity
{
    public int LessonSectionId { get; set; }
    public required int UserId { get; set; }
    public required double Value { get; set; }
}
