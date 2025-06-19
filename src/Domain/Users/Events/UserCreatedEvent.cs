using Domain.Common;

namespace Domain.Users.Events;

public class UserCreatedEvent : IDomainEvent
{
    public int UserId { get; }

    public UserCreatedEvent(int userId)
    {
        UserId = userId;
    }
}
