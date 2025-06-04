using System;

namespace Application.Common.Configuration;

public class UploadOptions
{
    public const string Uploads = "Uploads";
    public long MaxFileSizeBytes { get; set; }
}
