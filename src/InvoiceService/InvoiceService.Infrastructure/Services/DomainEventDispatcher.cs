using InvoiceService.Application.Services;
using InvoiceService.Domain.Abstractions;
using InvoiceService.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InvoiceService.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ApplicationDataContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        ApplicationDataContext context, 
        IMediator mediator, 
        ILogger<DomainEventDispatcher> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
    {
        var entities = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            
            foreach (var domainEvent in events)
            {
                try
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                    _logger.LogDebug("Domain event published: {EventType}", domainEvent.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro na publicação do evento de dominio: {EventType}", domainEvent.GetType().Name);
                }
            }
        }
    }
}