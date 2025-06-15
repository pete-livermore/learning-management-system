using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Domain.Enums;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Tests.UseCases.Users.Commands
{
    public class CreateUserCommandValidatorTests
    {
        private readonly AbstractValidator<CreateUserCommand> _validator =
            new CreateUserCommandValidator();

        [Fact]
        public void ShouldFail_WhenFirstNameIsEmpty()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CreateCommand.FirstName);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenLastNameIsEmpty()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "test",
                LastName = "",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CreateCommand.LastName);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenEmailIsInvalidFormat()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email/email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CreateCommand.Email);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenPasswordIsEmpty()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "",
                Role = UserRole.Administrator.ToString(),
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CreateCommand.Password);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenRoleIsInvalid()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = "hello",
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.CreateCommand.Role);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldPass_WhenDataIsValid()
        {
            var testDto = new CreateUserDto()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };

            var command = new CreateUserCommand() { CreateCommand = testDto };
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
            Assert.True(result.IsValid);
        }
    }
}
