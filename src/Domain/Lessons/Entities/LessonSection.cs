using System;
using Domain.Common;

namespace Domain.Lessons.Entities;

public class LessonSection : BaseEntity
{
    public int Id { get; private set; }
    public int Order { get; set; }
    public ICollection<LessonContent> Contents = [];
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}
