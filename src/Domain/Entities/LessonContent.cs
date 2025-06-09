namespace Domain.Entities;

public abstract class LessonContent : BaseEntity
{
    public int Id { get; private set; }
    public int Order { get; set; }
}

public enum MarkdownBlockType
{
    Paragraph,
    Heading,
    Code,
    Quote,
    List,
    Table,
}

public class MarkdownBlock
{
    public required MarkdownBlockType Type { get; set; }
    public string? Language { get; set; }
    public int? Level { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class MarkdownContent : LessonContent
{
    public required MarkdownBlock Markdown { get; set; }
}

public class MediaContent : LessonContent
{
    public required UploadFile File { get; set; }
}
