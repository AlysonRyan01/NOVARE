using InvoiceService.Application.Mappers.Customers;
using InvoiceService.Application.Queries.Customers;
using InvoiceService.Domain.Repositories.Customers;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Customers;

public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerDto>>>
{
    private readonly ICustomerQueryRepository _customerQueryRepository;

    public GetAllCustomersHandler(ICustomerQueryRepository customerQueryRepository)
    {
        _customerQueryRepository = customerQueryRepository;
    }

    public async Task<Result<IEnumerable<CustomerDto>>> Handle(
        GetAllCustomersQuery request, 
        CancellationToken cancellationToken)
    {
        var customersResult = await _customerQueryRepository.GetAllAsync();
        if (!customersResult.IsSuccess || customersResult.Value == null)
            return Result<IEnumerable<CustomerDto>>.Fail(customersResult.Errors!);
        
        var customers = customersResult.Value;
        
        var customersDto = customers.ToDto();
        
        return Result<IEnumerable<CustomerDto>>.Ok(customersDto);
    }
}