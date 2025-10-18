using InvoiceService.Application.Commands.Customers;
using InvoiceService.Application.Queries.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared.Dtos;

namespace InvoiceService.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/customers")
            .WithTags("Customers");
        
        group.MapGet("/", async (
            [FromServices] IMediator mediator) =>
        {
            var query = new GetAllCustomersQuery();
            var result = await mediator.Send(query);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("GetAllCustomers")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapGet("/{id:guid}", async (
            [FromServices] IMediator mediator,
            Guid id) =>
        {
            var query = new GetByIdQuery(id);
            var result = await mediator.Send(query);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.NotFound(result);
        })
        .WithName("GetCustomerById")
        .Produces<List<string>>(StatusCodes.Status404NotFound);
        
        group.MapPost("/", async (
            [FromServices] IMediator mediator,
            [FromBody] CreateCustomerCommand command) =>
        {
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("CreateCustomer")
        .Produces<CustomerDto>(StatusCodes.Status201Created)
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
    }
}