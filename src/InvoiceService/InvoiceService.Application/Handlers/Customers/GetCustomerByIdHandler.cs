using InvoiceService.Application.Mappers;
using InvoiceService.Application.Mappers.Customers;
using InvoiceService.Application.Queries.Customers;
using InvoiceService.Domain.Repositories.Customers;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Customers;

public class GetCustomerByIdHandler : IRequestHandler<GetByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerQueryRepository _customerQueryRepository;

    public GetCustomerByIdHandler(ICustomerQueryRepository customerQueryRepository)
    {
        _customerQueryRepository = customerQueryRepository;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var customerResult = await _customerQueryRepository.GetAsync(request.Id);
        if (!customerResult.IsSuccess || customerResult.Value == null)
            return Result<CustomerDto>.Fail(customerResult.Errors!);
        
        var customer = customerResult.Value;
        
        var customerDto = customer.ToDto();

        return Result<CustomerDto>.Ok(customerDto);
    }
}