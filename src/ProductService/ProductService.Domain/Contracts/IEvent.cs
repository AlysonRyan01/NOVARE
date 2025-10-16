using MediatR;

namespace ProductService.Domain.Contracts;

public interface IEvent : INotification
{
    DateTime OccurredOn { get; }
}