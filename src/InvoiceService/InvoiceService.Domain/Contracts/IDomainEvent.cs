using MediatR;

namespace InvoiceService.Domain.Contracts;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
} 
