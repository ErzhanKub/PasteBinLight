namespace Domain.Abstractions;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _events = new();
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
    public List<IDomainEvent> Events => _events.ToList();
    protected void Raise(IDomainEvent domainEvent)
    {
        Events.Add(domainEvent);
    }
}

public interface IDomainEvent { }
