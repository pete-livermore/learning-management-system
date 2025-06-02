namespace Application.Errors;

public static class AuthErrors
{
    public static Error InvalidUser(string email) =>
        new("Auth.InvalidUser", $"The user with email '{email}' is not an authenticated user");

    public static Error InvalidPassword(string email, string password) =>
        new("Auth.InvalidUser", $"The password '{password}' for user {email} is not correct");
}
