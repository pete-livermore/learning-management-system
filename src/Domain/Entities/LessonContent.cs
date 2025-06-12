using Domain.ValueObjects;

namespace Domain.Entities;

public abstract class LessonContent : BaseEntity
{
    public int Id { get; private set; }
    public int Order { get; set; }
}

public class MarkdownContent : LessonContent
{
    public required MarkdownBlock Markdown { get; set; }
}

public class MediaContent : LessonContent
{
    public required UploadFile File { get; set; }
}
