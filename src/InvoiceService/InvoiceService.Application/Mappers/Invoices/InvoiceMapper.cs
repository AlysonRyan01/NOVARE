using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Entities;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Mappers.Invoices;

public static class InvoiceMapper
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto(
            Id: invoice.Id,
            Number: invoice.Number.Value,
            Status: invoice.Status.ToString(),
            CustomerId: invoice.CustomerId,
            Total: invoice.Total,
            CreatedAt: invoice.CreatedAt,
            PrintedAt: invoice.PrintedAt,
            Errors: invoice.Errors,
            Items: invoice.Items.Select(item => item.ToDto()).ToList()
        );
    }

    public static List<InvoiceDto> ToDto(this IEnumerable<Invoice> invoices)
    {
        return invoices.Select(invoice => invoice.ToDto()).ToList();
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item)
    {
        return new InvoiceItemDto(
            ProductId: item.ProductId,
            ProductName: item.ProductName,
            Quantity: item.Quantity,
            UnitPrice: item.UnitPrice
        );
    }
}