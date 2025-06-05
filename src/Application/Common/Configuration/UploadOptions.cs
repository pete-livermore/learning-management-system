namespace Application.Common.Configuration;

public class UploadOptions
{
    public const string Uploads = "Uploads";
    public long MaxFileSizeBytes { get; set; } = 1024 * 1000;
}
