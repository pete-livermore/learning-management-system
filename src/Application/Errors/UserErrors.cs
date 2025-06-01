namespace Application.Errors
{
    public static class UserErrors
    {
        public enum Code
        {
            NotFound,
            Conflict,
            Unauthorized,
        }

        public static Error NotFound(int id) =>
            new(Code.NotFound.ToString(), $"The user with Id '{id}' was not found");

        public static Error NotFound(string email) =>
            new(Code.NotFound.ToString(), $"The user with email '{email}' was not found");

        public static Error Conflict(string email) =>
            new(Code.Conflict.ToString(), $"The user with email '{email}' already exists");

        public static Error Unauthorized() =>
            new(
                Code.Unauthorized.ToString(),
                $"The current user is not authorised to access this resource"
            );
    }
}
