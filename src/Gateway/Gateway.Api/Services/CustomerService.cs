using System.Text;
using System.Text.Json;
using Gateway.Api.Interfaces;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InvoiceService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public CustomerService(IHttpClientFactory httpClientFactory, ILogger<InvoiceService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("CustomerService");
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(createCustomerDto, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/customers", content);
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<CustomerDto>>();
            if (apiResponse == null)
                return Result<CustomerDto>.Fail(["Erro ao criar cliente"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            return Result<CustomerDto>.Fail(["Erro ao criar cliente"]);
        }
    }

    public async Task<Result<CustomerDto>> GetCustomerByIdAsync(Guid customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/{customerId}");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<CustomerDto>>();
            if (apiResponse == null)
                return Result<CustomerDto>.Fail(["Erro ao buscar cliente"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente por ID: {CustomerId}", customerId);
            return Result<CustomerDto>.Fail(["Erro ao buscar cliente"]);
        }
    }

    public async Task<Result<IEnumerable<CustomerDto>>> GetCustomersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/customers");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<IEnumerable<CustomerDto>>>();
            if (apiResponse == null)
                return Result<IEnumerable<CustomerDto>>.Fail(["Erro ao buscar clientes"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar clientes");
            return Result<IEnumerable<CustomerDto>>.Fail(["Erro ao buscar clientes"]);
        }
    }
}