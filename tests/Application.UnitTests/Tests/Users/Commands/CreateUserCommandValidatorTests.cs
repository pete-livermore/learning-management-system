using Application.Users.Commands;
using Domain.Users.Enums;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Tests.Users.Commands
{
    public class CreateUserCommandValidatorTests
    {
        private readonly AbstractValidator<CreateUserCommand> _validator =
            new CreateUserCommandValidator();

        [Fact]
        public void ShouldFail_WhenFirstNameIsEmpty()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.FirstName);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenLastNameIsEmpty()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "test",
                LastName = "",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.LastName);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenEmailIsInvalidFormat()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email/email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Email);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenPasswordIsEmpty()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "",
                Role = UserRole.Administrator.ToString(),
            };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Password);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldFail_WhenRoleIsInvalid()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = "hello",
            };
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Role);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldPass_WhenDataIsValid()
        {
            var command = new CreateUserCommand()
            {
                FirstName = "test",
                LastName = "user",
                Email = "test_email@email.com",
                Password = "password",
                Role = UserRole.Administrator.ToString(),
            };
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
            Assert.True(result.IsValid);
        }
    }
}
