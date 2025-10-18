using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Queries.Invoices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared.Dtos;

namespace InvoiceService.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/invoices")
            .WithTags("Invoices");
        
        group.MapGet("/", async (
            [FromServices] IMediator mediator,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var query = new GetAllQuery(pageNumber, pageSize);
            var result = await mediator.Send(query);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("GetAllInvoices")
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
        .WithName("GetInvoiceById")
        .Produces<List<string>>(StatusCodes.Status404NotFound);
        
        group.MapPost("/", async (
            [FromServices] IMediator mediator,
            [FromBody] CreateInvoiceCommand command) =>
        {
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("CreateInvoice")
        .Produces<InvoiceDto>(StatusCodes.Status201Created)
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapPut("/{id:guid}", async (
            [FromServices] IMediator mediator,
            Guid id,
            [FromBody] UpdateInvoiceCommand command) =>
        {
            var updateCommand = command with { InvoiceId = id };
            var result = await mediator.Send(updateCommand);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("UpdateInvoice")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapDelete("/{id:guid}", async (
            [FromServices] IMediator mediator,
            Guid id) =>
        {
            var command = new DeleteInvoiceCommand(id);
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.NoContent()
                : Results.BadRequest(result.Errors);
        })
        .WithName("DeleteInvoice")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapPost("/{id:guid}/print", async (
            [FromServices] IMediator mediator,
            Guid id) =>
        {
            var command = new RequestPrintCommand(id);
            var result = await mediator.Send(command);
            
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("RequestInvoicePrint")
        .Produces<List<string>>(StatusCodes.Status400BadRequest);
        
        group.MapGet("/{id:guid}/status", async (
            [FromServices] IMediator mediator,
            Guid id) =>
        {
            var query = new GetByIdQuery(id);
            var result = await mediator.Send(query);
            
            if (!result.IsSuccess || result.Value == null)
                return Results.NotFound(result.Errors);

            var invoice = result.Value;
            var statusResponse = new
            {
                invoice.Id,
                invoice.Status,
                invoice.PrintedAt,
                invoice.Errors,
                Timestamp = DateTime.UtcNow
            };
            
            return Results.Ok(statusResponse);
        })
        .WithName("GetInvoiceStatus")
        .Produces(StatusCodes.Status200OK)
        .Produces<List<string>>(StatusCodes.Status404NotFound);
    }
}