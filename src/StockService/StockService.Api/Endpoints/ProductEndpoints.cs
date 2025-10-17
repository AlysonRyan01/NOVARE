using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Queries;

namespace ProductService.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var productGroup = app.MapGroup("/api/products")
            .WithTags("Products");

        productGroup.MapGet("/", async (
                IMediator mediator,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 50) =>
            {
                var query = new GetAllQuery(pageNumber, pageSize);
                var result = await mediator.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("GetAllProducts")
            .WithSummary("Get all products with pagination")
            .WithDescription("Retrieve a paginated list of all products");

        productGroup.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var query = new GetByIdQuery(id);
                var result = await mediator.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .WithDescription("Retrieve a specific product by its unique identifier");

        productGroup.MapPost("/", async (IMediator mediator, CreateProductCommand command) =>
            {
                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Create a new product with the provided details");

        productGroup.MapPut("/{id:guid}", async (IMediator mediator, Guid id, UpdateProductCommand command) =>
            {
                if (id != command.Id)
                    return Results.BadRequest(Result<string>.Fail(["O ID do produto não corresponde a rota"]));

                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .WithDescription("Update the details of an existing product");

        productGroup.MapDelete("/{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var command = new DeleteProductCommand(id);
                var result = await mediator.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .WithDescription("Delete a specific product by its unique identifier");

        productGroup.MapPost("/{id:guid}/increase-stock",
                async (IMediator mediator, Guid id, IncreaseStockCommand command) =>
                {
                    if (id != command.ProductId)
                        return Results.BadRequest(Result<string>.Fail(["O ID do produto não corresponde a rota"]));

                    var result = await mediator.Send(command);

                    return result.IsSuccess
                        ? Results.Ok(result)
                        : Results.BadRequest(result);
                })
            .WithName("IncreaseStock")
            .WithSummary("Increase product stock")
            .WithDescription("Increase the stock quantity of a specific product");

        productGroup.MapPost("/{id:guid}/decrease-stock",
                async (IMediator mediator, Guid id, DecreaseStockCommand command) =>
                {
                    if (id != command.ProductId)
                        return Results.BadRequest(Result<string>.Fail(["O ID do produto não corresponde a rota"]));

                    var result = await mediator.Send(command);

                    return result.IsSuccess
                        ? Results.Ok(result)
                        : Results.BadRequest(result);
                })
            .WithName("DecreaseStock")
            .WithSummary("Decrease product stock")
            .WithDescription("Decrease the stock quantity of a specific product");

        productGroup.MapGet("/health",
                () => { return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }); })
            .WithName("HealthCheck")
            .WithSummary("Health check")
            .WithDescription("Check if the products API is running properly")
            .ExcludeFromDescription();
    }
}