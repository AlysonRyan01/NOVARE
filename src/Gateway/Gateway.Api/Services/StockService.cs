using Gateway.Api.Interfaces;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Services;

public class StockService : IStockService
{
    private readonly HttpClient _httpClient;

    public StockService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockService");
    }

    public async Task<Result<IEnumerable<ProductDto>>> GetAllProducts(int pageNumber, int pageSize)
    {
        var response = await _httpClient.GetAsync($"/api/products?pageNumber={pageNumber}&pageSize={pageSize}");
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ProductDto>>>();
        if (apiResponse == null)
            return Result<IEnumerable<ProductDto>>.Fail(["Erro ao buscar os produtos"]);
        
        return apiResponse;
    }

    public async Task<Result<ProductDto>> GetProductById(Guid productId)
    {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        if (apiResponse == null)
            return Result<ProductDto>.Fail(["Erro ao buscar o produto"]);
        
        return apiResponse;
    }

    public async Task<Result<ProductDto>> CreateProduct(CreateProductDto productDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/products", productDto);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        if (apiResponse == null)
            return Result<ProductDto>.Fail(["Erro ao cadastrar produto"]);
        
        return apiResponse;
    }

    public async Task<Result<ProductDto>> UpdateProduct(UpdateProductDto productDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/products/{productDto.Id}", productDto);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        if (apiResponse == null)
            return Result<ProductDto>.Fail(["Erro ao atualizar produto"]);
        
        return apiResponse;
    }

    public async Task<Result<Guid>> DeleteProduct(Guid productId)
    {
        var response = await _httpClient.DeleteAsync($"/api/products/{productId}");
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        if (apiResponse == null)
            return Result<Guid>.Fail(["Erro ao deletar produto"]);
        
        return apiResponse;
    }

    public async Task<Result<ProductDto>> IncreaseStock(IncreaseStockDto increaseStockDto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/products/{increaseStockDto.ProductId}/increase-stock", increaseStockDto);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        if (apiResponse == null)
            return Result<ProductDto>.Fail(["Erro ao aumentar estoque"]);
        
        return apiResponse;
    }

    public async Task<Result<ProductDto>> DecreaseStock(DecreaseStockDto decreaseStockDto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/products/{decreaseStockDto.ProductId}/decrease-stock", decreaseStockDto);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        if (apiResponse == null)
            return Result<ProductDto>.Fail(["Erro ao diminuir estoque"]);
        
        return apiResponse;
    }
}