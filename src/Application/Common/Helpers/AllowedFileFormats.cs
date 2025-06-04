namespace Application.Common.Constants;

public sealed record FileFormat(string Extension, string MimeType);

public static class AllowedFileFormats
{
    public static readonly IReadOnlyList<FileFormat> All = new[]
    {
        new FileFormat("txt", "text/plain"),
        new FileFormat("jpg", "image/jpeg"),
        new FileFormat("png", "image/png"),
        new FileFormat("mp4", "video/mp4"),
        new FileFormat("webp", "image/webp"),
        new FileFormat("mpg", "video/mpeg"),
        new FileFormat("pdf", "application/pdf"),
    };

    public static bool IsExtensionAllowed(string ext) => All.Any(f => f.Extension == ext);

    public static string? GetMimeType(string ext) =>
        All.FirstOrDefault(f => f.Extension == ext)?.MimeType;
}
