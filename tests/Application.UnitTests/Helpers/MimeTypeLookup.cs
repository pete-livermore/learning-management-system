using Microsoft.AspNetCore.StaticFiles;

namespace Application.UnitTests.Helpers;

public class MimeTypeLookup
{
    public static string GetMimeTypeForFileExtension(string filePath)
    {
        const string DefaultContentType = "application/octet-stream";

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out string contentType))
        {
            contentType = DefaultContentType;
        }

        return contentType;
    }
}
