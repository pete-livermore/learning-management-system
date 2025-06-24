using Domain.Common;
using Domain.Uploads.Entities;

namespace Domain.Lessons.Entities;

public abstract class LessonSectionContent : BaseEntity
{
    public int Id { get; private set; }
    public int Order { get; set; }
}

public class MarkdownContent : LessonSectionContent
{
    public required MarkdownBlock Markdown { get; set; }
}

public class MediaContent : LessonSectionContent
{
    public required UploadFile File { get; set; }
}
