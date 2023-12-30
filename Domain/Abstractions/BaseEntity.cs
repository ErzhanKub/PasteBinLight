namespace Domain.Abstractions;

// Abstract base class for entities
public abstract class BaseEntity
{
    // List of domain events
    private readonly List<IDomainEvent> _domainEvents = new();

    // Constructor
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    // Unique identifier
    public Guid Id { get; init; }

    // Domain events associated with this entity
    public List<IDomainEvent> DomainEvents => _domainEvents.ToList();

    // Method to raise a new domain event
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }
}

// Interface for domain events
public interface IDomainEvent { }
