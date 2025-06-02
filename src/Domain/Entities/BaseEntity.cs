namespace Domain.Entities;

public abstract class BaseEntity
{
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

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
