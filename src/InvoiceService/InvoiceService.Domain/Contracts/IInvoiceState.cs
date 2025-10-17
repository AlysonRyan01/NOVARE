using InvoiceService.Domain.AggregateRoots;
using SharedService.Shared;

namespace InvoiceService.Domain.Contracts;

public interface IInvoiceState
{
    Result<Invoice> Pending(Invoice invoice);
    Result<Invoice> ValidationRequested(Invoice invoice);
    Result<Invoice> OutOfStock(Invoice invoice);
    Result<Invoice> Printed(Invoice invoice);
}