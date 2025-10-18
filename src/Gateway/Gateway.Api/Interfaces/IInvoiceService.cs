using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Interfaces;

public interface IInvoiceService
{
    Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<Result<CustomerDto>> GetCustomerByIdAsync(Guid customerId);
    Task<Result<IEnumerable<CustomerDto>>> GetCustomersAsync();
    
    Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);
    Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid invoiceId);
    Task<Result<IEnumerable<InvoiceDto>>> GetInvoicesAsync(int pageNumber = 1, int pageSize = 20);
    Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDto updateInvoiceDto);
    Task<Result<Guid>> DeleteInvoiceAsync(Guid invoiceId);
    Task<Result<InvoiceDto>> RequestPrintAsync(Guid invoiceId);
    Task<Result<InvoiceStatusDto>> GetInvoiceStatusAsync(Guid invoiceId);
}