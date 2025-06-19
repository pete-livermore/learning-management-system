using Application.Common.Errors;
using Application.Common.Errors.Factories;

namespace Application.Users.Errors;

public static class UserErrors
{
    public static Error NotFound(int userId) =>
        ResourceError.NotFound($"The user with {userId} was not found");

    public static Error Forbidden() =>
        SecurityError.Forbidden(
            "The current application user is forbidden from accessing this user resource"
        );
}
