using Application.UseCases.Security.Dtos;

namespace Application.UseCases.Security.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUserDto GetCurrentUser();
}
