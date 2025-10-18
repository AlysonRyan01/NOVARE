using Gateway.Api.Interfaces;
using SharedService.Shared;
using SharedService.Shared.Dtos;
using System.Text;
using System.Text.Json;

namespace Gateway.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InvoiceService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public InvoiceService(IHttpClientFactory httpClientFactory, ILogger<InvoiceService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("InvoiceService");
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

    public async Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(createInvoiceDto, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/invoices", content);
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<InvoiceDto>>();
            if (apiResponse == null)
                return Result<InvoiceDto>.Fail(["Erro ao criar nota fiscal"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar nota fiscal");
            return Result<InvoiceDto>.Fail(["Erro ao criar nota fiscal"]);
        }
    }

    public async Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/invoices/{invoiceId}");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<InvoiceDto>>();
            if (apiResponse == null)
                return Result<InvoiceDto>.Fail(["Erro ao buscar nota fiscal"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar nota fiscal por ID: {InvoiceId}", invoiceId);
            return Result<InvoiceDto>.Fail(["Erro ao buscar nota fiscal"]);
        }
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> GetInvoicesAsync(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/invoices?pageNumber={pageNumber}&pageSize={pageSize}");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<IEnumerable<InvoiceDto>>>();
            if (apiResponse == null)
                return Result<IEnumerable<InvoiceDto>>.Fail(["Erro ao buscar notas fiscais"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar notas fiscais. Página: {PageNumber}, Tamanho: {PageSize}", pageNumber, pageSize);
            return Result<IEnumerable<InvoiceDto>>.Fail(["Erro ao buscar notas fiscais"]);
        }
    }

    public async Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDto updateInvoiceDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(updateInvoiceDto, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/invoices/{invoiceId}", content);
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<InvoiceDto>>();
            if (apiResponse == null)
                return Result<InvoiceDto>.Fail(["Erro ao atualizar nota fiscal"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar nota fiscal: {InvoiceId}", invoiceId);
            return Result<InvoiceDto>.Fail(["Erro ao atualizar nota fiscal"]);
        }
    }

    public async Task<Result<Guid>> DeleteInvoiceAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/invoices/{invoiceId}");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<Guid>>();
            if (apiResponse == null)
                return Result<Guid>.Fail(["Erro ao excluir nota fiscal"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir nota fiscal: {InvoiceId}", invoiceId);
            return Result<Guid>.Fail(["Erro ao excluir nota fiscal"]);
        }
    }

    public async Task<Result<InvoiceDto>> RequestPrintAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/invoices/{invoiceId}/print", null);
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<InvoiceDto>>();
            if (apiResponse == null)
                return Result<InvoiceDto>.Fail(["Erro ao solicitar impressão"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao solicitar impressão da nota fiscal: {InvoiceId}", invoiceId);
            return Result<InvoiceDto>.Fail(["Erro ao solicitar impressão"]);
        }
    }

    public async Task<Result<InvoiceStatusDto>> GetInvoiceStatusAsync(Guid invoiceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/invoices/{invoiceId}/status");
            
            var apiResponse = await response.Content.ReadFromJsonAsync<Result<InvoiceStatusDto>>();
            if (apiResponse == null)
                return Result<InvoiceStatusDto>.Fail(["Erro ao buscar status da nota fiscal"]);
            
            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar status da nota fiscal: {InvoiceId}", invoiceId);
            return Result<InvoiceStatusDto>.Fail(["Erro ao buscar status da nota fiscal"]);
        }
    }
}