using Application.Security.Dtos;

namespace Application.Security.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUserDto GetCurrentUser();
}
