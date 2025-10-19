using Gateway.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Endpoints;

public static class InvoiceGatewayEndpoints
{
    public static void MapInvoiceGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api")
            .WithTags("Gateway - Invoice Service");

        var invoicesGroup = group.MapGroup("/invoices");

        invoicesGroup.MapGet("/", async (
                [FromServices] IInvoiceService invoiceService,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20) =>
            {
                var result = await invoiceService.GetInvoicesAsync(pageNumber, pageSize);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("GetAllInvoices")
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        invoicesGroup.MapGet("/{id:guid}", async (
                [FromServices] IInvoiceService invoiceService,
                Guid id) =>
            {
                var result = await invoiceService.GetInvoiceByIdAsync(id);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("GetInvoiceById")
            .Produces<List<string>>(StatusCodes.Status404NotFound);

        invoicesGroup.MapPost("/", async (
                [FromServices] IInvoiceService invoiceService,
                [FromBody] CreateInvoiceDto createInvoiceDto) =>
            {
                var result = await invoiceService.CreateInvoiceAsync(createInvoiceDto);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("CreateInvoice")
            .Produces<InvoiceDto>(StatusCodes.Status201Created)
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        invoicesGroup.MapPut("/{id:guid}", async (
                [FromServices] IInvoiceService invoiceService,
                Guid id,
                [FromBody] UpdateInvoiceDto updateInvoiceDto) =>
            {
                var result = await invoiceService.UpdateInvoiceAsync(id, updateInvoiceDto);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("UpdateInvoice")
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        invoicesGroup.MapDelete("/{id:guid}", async (
                [FromServices] IInvoiceService invoiceService,
                Guid id) =>
            {
                var result = await invoiceService.DeleteInvoiceAsync(id);
                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Errors);
            })
            .WithName("DeleteInvoice")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        invoicesGroup.MapPost("/{id:guid}/print", async (
                [FromServices] IInvoiceService invoiceService,
                Guid id) =>
            {
                var result = await invoiceService.RequestPrintAsync(id);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("RequestInvoicePrint")
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        invoicesGroup.MapGet("/{id:guid}/status", async (
                [FromServices] IInvoiceService invoiceService,
                Guid id) =>
            {
                var result = await invoiceService.GetInvoiceStatusAsync(id);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.NotFound(result.Errors);
            })
            .WithName("GetInvoiceStatus")
            .Produces<List<string>>(StatusCodes.Status404NotFound);
    }
}