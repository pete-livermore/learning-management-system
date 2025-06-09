using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTests.Helpers.Factories
{
    public record CreateFileOptions
    {
        public required string FileName { get; init; }
        public required long FileSize { get; init; }
        public string Content { get; init; } = "";
    }

    public class MockFileFactory
    {
        public static IFormFile Create(string ext, CreateFileOptions createOptions)
        {
            var fileMock = new Mock<IFormFile>();
            var fullFileName = createOptions.FileName + ext;

            fileMock.Setup(_ => _.FileName).Returns(createOptions.FileName);
            fileMock
                .Setup(_ => _.ContentType)
                .Returns(MimeTypeLookup.GetMimeTypeForFileExtension(ext));
            fileMock.Setup(_ => _.Length).Returns(createOptions.FileSize);

            if (ext == ".txt")
            {
                var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(createOptions.Content));
                fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
                fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);
            }

            return fileMock.Object;
        }
    }
}
