using SharedService.Shared;

namespace InvoiceService.Domain.Contracts;

public interface IBuilder<T>
{
    Result<T> Build();
}