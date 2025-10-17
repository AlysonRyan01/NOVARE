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
        
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ProductDto>>>();
            if (products == null)
                return Result<IEnumerable<ProductDto>>.Fail(["Erro ao buscar os produtos"]);
            
            return products;
        }
        
        return Result<IEnumerable<ProductDto>>.Fail(["Erro ao buscar produtos"]);
    }

    public async Task<Result<ProductDto>> GetProductById(Guid productId)
    {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");
        
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
            if (product == null)
                return Result<ProductDto>.Fail(["Erro ao buscar o produto"]);
            
            return product;
        }
        
        return Result<ProductDto>.Fail(["Produto não encontrado"]);
    }

    public async Task<Result<ProductDto>> CreateProduct(CreateProductDto productDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/products", productDto);
        
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
            if (product == null)
                return Result<ProductDto>.Fail(["Erro ao criar o produto"]);
            
            return product;
        }
        
        return Result<ProductDto>.Fail(["Erro ao criar produto"]);
    }

    public async Task<Result<ProductDto>> UpdateProduct(UpdateProductDto productDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/products/{productDto.Id}", productDto);
        
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
            if (product == null)
                return Result<ProductDto>.Fail(["Erro ao atualizar o produto"]);
            
            return product;
        }
        
        return Result<ProductDto>.Fail(["Erro ao atualizar produto"]);
    }

    public async Task<Result<Guid>> DeleteProduct(Guid productId)
    {
        var response = await _httpClient.DeleteAsync($"/api/products/{productId}");
        
        if (response.IsSuccessStatusCode)
        {
            var id = await response.Content.ReadFromJsonAsync<Result<Guid>>();
            if (id == null)
                return Result<Guid>.Fail(["Erro ao buscar o produto"]);

            return id;
        }
        
        return Result<Guid>.Fail(["Erro ao deletar produto"]);
    }

    public async Task<Result<ProductDto>> IncreaseStock(IncreaseStockDto increaseStockDto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/products/{increaseStockDto.ProductId}/increase-stock", increaseStockDto);
        
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
            if (product == null)
                return Result<ProductDto>.Fail(["Erro ao atualizar o produto"]);
            
            return product;
        }
        
        return Result<ProductDto>.Fail(["Erro ao aumentar estoque"]);
    }

    public async Task<Result<ProductDto>> DecreaseStock(DecreaseStockDto decreaseStockDto)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/products/{decreaseStockDto.ProductId}/decrease-stock", decreaseStockDto);
        
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
            if (product == null)
                return Result<ProductDto>.Fail(["Erro ao atualizar o produto"]);
            
            return product;
        }
        
        return Result<ProductDto>.Fail(["Erro ao diminuir estoque"]);
    }
}