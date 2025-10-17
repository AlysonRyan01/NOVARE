using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Contracts;
using SharedService.Shared;

namespace InvoiceService.Domain.States.Invoices;

public class PrintedState : IInvoiceState
{
    public Result<Invoice> Pending(Invoice invoice)
        => Result<Invoice>.Fail(["Uma nota fiscal com impressa não pode voltar a ser pendente"]);

    public Result<Invoice> ValidationRequested(Invoice invoice)
        => Result<Invoice>.Fail(["Uma nota fiscal impressa já foi validada"]);

    public Result<Invoice> OutOfStock(Invoice invoice)
        => Result<Invoice>.Fail(["Uma nota fiscal com impressa não pode ter produtos fora de estoque"]);

    public Result<Invoice> Printed(Invoice invoice)
        => Result<Invoice>.Fail(["A nota fiscal já está com status 'impressa'"]);
}