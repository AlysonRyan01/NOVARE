using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Enums;
using SharedService.Shared;

namespace InvoiceService.Domain.States.Invoices;

public class ValidationRequestedState : IInvoiceState
{
    public Result<Invoice> Pending(Invoice invoice)
        => Result<Invoice>.Fail(["Voce não pode adicionar status pendente em uma nota fiscal aguardando validação"]);

    public Result<Invoice> ValidationRequested(Invoice invoice)
        => Result<Invoice>.Fail(["Voce já solicitou a validação da nota fiscal"]);

    public Result<Invoice> OutOfStock(Invoice invoice)
    {
        invoice.Status = EInvoiceStatus.OutOfStock;
        invoice.ChangeState(new OutOfStockState());
        
        return Result<Invoice>.Ok(invoice);
    }

    public Result<Invoice> Printed(Invoice invoice)
    {
        invoice.Status = EInvoiceStatus.Printed;
        invoice.ChangeState(new PrintedState());
        invoice.PrintedAt = DateTime.UtcNow;
        
        return Result<Invoice>.Ok(invoice);
    }
}