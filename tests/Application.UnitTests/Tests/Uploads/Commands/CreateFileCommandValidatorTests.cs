using Application.UnitTests.Helpers.Factories;
using Application.Uploads.Commands;
using Application.Uploads.Dtos;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Tests.Uploads.Commands
{
    public class CreateFileCommandValidatorTests
    {
        [Fact]
        public void ShouldPass_WhenCommandIsValid()
        {
            var createFileCommand = new CreateFileCommand()
            {
                FileContent = MockFileFactory.Create(
                    ".mpg",
                    new CreateFileOptions() { FileName = "test_file", FileSize = 2000 }
                ),
                FileMetadata = new FileMetadataDto(),
            };
            var validator = new CreateFileCommandValidator();

            var result = validator.Validate(createFileCommand);

            Assert.True(result.IsValid);
        }
    }
}
