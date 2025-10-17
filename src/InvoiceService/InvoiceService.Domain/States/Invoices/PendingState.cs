using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Enums;
using SharedService.Shared;

namespace InvoiceService.Domain.States.Invoices;

public class PendingState : IInvoiceState
{
    public Result<Invoice> Pending(Invoice invoice)
        => Result<Invoice>.Fail(["A ordem já está com status pendente"]);

    public Result<Invoice> ValidationRequested(Invoice invoice)
    {
        invoice.Status = EInvoiceStatus.ValidationRequested;
        invoice.ChangeState(new ValidationRequestedState());
        
        return Result<Invoice>.Ok(invoice);
    }

    public Result<Invoice> OutOfStock(Invoice invoice)
        => Result<Invoice>.Fail(["Voce precisa passar pela validação antes de marcar como 'produt fora de estoque'"]);

    public Result<Invoice> Printed(Invoice invoice)
        => Result<Invoice>.Fail(["Voce precisa passar pela validação antes de marcar como impresso"]);
}