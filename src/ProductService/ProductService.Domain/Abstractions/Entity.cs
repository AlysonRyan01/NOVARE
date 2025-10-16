using ProductService.Domain.Contracts;

namespace ProductService.Domain.Abstractions;

public abstract class Entity 
{
    public Guid Id { get; protected set; }
    
    private readonly List<IEvent> _domainEvents  = new();
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}