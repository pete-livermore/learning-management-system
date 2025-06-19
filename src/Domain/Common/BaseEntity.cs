using MediatR;

namespace Domain.Common;

public abstract class BaseEntity
{
    private readonly List<INotification> _domainEvents = [];
    public List<INotification> DomainEvents => _domainEvents;
    private DateTime _updatedAt = DateTime.UtcNow;

    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (value > DateTime.UtcNow)
            {
                throw new ArgumentException("Updated date cannot be in the future");
            }
            _updatedAt = value;
        }
    }

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
