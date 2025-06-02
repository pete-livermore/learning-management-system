namespace Application.Common.Interfaces.Cache;

public interface ICacheService
{
    public Task<T?> GetValueAsync<T>(string key, CancellationToken cancellationToken = default);
    public Task SetValueAsync<T>(string key, T data);
}
