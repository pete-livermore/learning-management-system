using Domain.Enums;

namespace Domain.ValueObjects;

public class MarkdownBlock
{
    public required MarkdownBlockType Type { get; set; }
    public string? Language { get; set; }
    public int? Level { get; set; }
    public string Content { get; set; } = string.Empty;
}
