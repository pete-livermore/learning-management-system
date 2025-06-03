using Application.Common.Errors;

namespace Application.UseCases.Users.Errors
{
    public static class UserErrors
    {
        private const string ErrorContext = "Users";
        public const string NotFoundCode = $"{ErrorContext}.NotFound";
        public const string ConflictCode = $"{ErrorContext}.Conflict";
        public const string UnexpectedCode = $"{ErrorContext}.Unexpected";

        public static Error NotFound(int id) =>
            new(ErrorType.NotFound, NotFoundCode, $"The user with Id '{id}' was not found");

        public static Error NotFound(string email) =>
            new(ErrorType.NotFound, NotFoundCode, $"The user with email '{email}' was not found");

        public static Error Conflict(string email) =>
            new(ErrorType.Conflict, ConflictCode, $"The user with email '{email}' already exists");

        public static Error Unexpected(string message) =>
            new(ErrorType.Unexpected, UnexpectedCode, message);
    }
}
