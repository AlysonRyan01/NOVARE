using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<Result<CustomerDto>> GetCustomerByIdAsync(Guid customerId);
    Task<Result<IEnumerable<CustomerDto>>> GetCustomersAsync();
}