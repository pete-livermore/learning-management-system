using Application.Common.Interfaces.Repositories;
using Application.UnitTests.Helpers.Factories;
using Application.UseCases.Security.Interfaces;
using Application.UseCases.Uploads.Commands;
using Application.UseCases.Uploads.Interfaces;
using Moq;

namespace Application.UnitTests.UseCases.Uploads.Commands
{
    public class CreateFileCommandTests
    {
        private readonly Mock<IFileValidator> _fileValidatorMock = new();
        private readonly Mock<IUploadsService> _uploadsServiceMock = new();
        private readonly Mock<IFilesRepository> _filesRepositoryMock = new();
        private readonly Mock<ICurrentUserProvider> _userAccessorMock = new();

        public CreateFileCommandHandler CreateHandler()
        {
            return new CreateFileCommandHandler(
                _fileValidatorMock.Object,
                _filesRepositoryMock.Object,
                _uploadsServiceMock.Object,
                _userAccessorMock.Object
            );
        }

        [Fact]
        public async void ShouldReturnFailureResult_WhenResourceTypeIsInvalid()
        {
            var handler = CreateHandler();
            var testFile = MockFileFactory.Create(
                ".jpg",
                new CreateFileOptions() { FileName = "randomfile", FileSize = 1000 }
            );
            var command = new CreateFileCommand() { FileContent = testFile };
            _fileValidatorMock.Setup(fv => fv.IsValidFile(testFile)).Returns(false);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal("Invalid file", result.Errors[0].Description);
        }
    }
}
