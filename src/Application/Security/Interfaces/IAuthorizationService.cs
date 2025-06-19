namespace Application.Security.Interfaces;

public interface IAuthorizationService
{
    public bool GetIsAuthorized(Guid targetOwnerId);
}
