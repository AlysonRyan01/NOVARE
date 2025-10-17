using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Enums;
using SharedService.Shared;

namespace InvoiceService.Domain.States.Invoices;

public class OutOfStockState : IInvoiceState
{
    public Result<Invoice> Pending(Invoice invoice)
        => Result<Invoice>.Fail(["Uma nota fiscal com produto fora de estoque não pode se tornar pendente"]);

    public Result<Invoice> ValidationRequested(Invoice invoice)
    {
        invoice.Status = EInvoiceStatus.ValidationRequested;
        invoice.ChangeState(new ValidationRequestedState());
        
        return Result<Invoice>.Ok(invoice);
    }

    public Result<Invoice> OutOfStock(Invoice invoice)
        => Result<Invoice>.Fail(["A nota fiscal já está com status 'produto fora de estoque'"]);

    public Result<Invoice> Printed(Invoice invoice)
        => Result<Invoice>.Fail(["Voce precisa passar pela validação antes de imprimir"]);
}