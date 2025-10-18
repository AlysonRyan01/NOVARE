using Gateway.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Endpoints;

public static class StockGatewayEndpoints
{
    public static void MapStockEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Gateway - Stock Service");
        
        group.MapGet("/", async (
            [FromServices] IStockService stockService,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var result = await stockService.GetAllProducts(pageNumber, pageSize);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("GetAllProducts")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapGet("/{id:guid}", async (
            [FromServices] IStockService stockService,
            Guid id) =>
        {
            var result = await stockService.GetProductById(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.NotFound(result);
        })
        .WithName("GetProductById")
        .Produces<List<string>>(StatusCodes.Status404NotFound);
        
        group.MapPost("/", async (
            [FromServices] IStockService stockService,
            [FromBody] CreateProductDto createProductDto) =>
        {
            var result = await stockService.CreateProduct(createProductDto);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("CreateProduct")
        .Produces<ProductDto>(StatusCodes.Status201Created)
        .Produces<List<string>>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (
            [FromServices] IStockService stockService,
            Guid id,
            [FromBody] UpdateProductDto updateProductDto) =>
        {
            var command = updateProductDto with { Id = id };
            var result = await stockService.UpdateProduct(command);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("UpdateProduct")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapDelete("/{id:guid}", async (
            [FromServices] IStockService stockService,
            Guid id) =>
        {
            var result = await stockService.DeleteProduct(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapPatch("/{id:guid}/increase-stock", async (
            [FromServices] IStockService stockService,
            Guid id,
            [FromBody] IncreaseStockRequest request) =>
        {
            var increaseStockDto = new IncreaseStockDto(id, request.Quantity);
            var result = await stockService.IncreaseStock(increaseStockDto);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("IncreaseStock")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapPatch("/{id:guid}/decrease-stock", async (
            [FromServices] IStockService stockService,
            Guid id,
            [FromBody] DecreaseStockRequest request) =>
        {
            var decreaseStockDto = new DecreaseStockDto(id, request.Quantity);
            var result = await stockService.DecreaseStock(decreaseStockDto);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("DecreaseStock")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
    }

}