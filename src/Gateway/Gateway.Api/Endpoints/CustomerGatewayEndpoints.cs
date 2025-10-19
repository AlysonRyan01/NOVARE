using Gateway.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Endpoints;

public static class CustomerGatewayEndpoints
{
    public static void MapCustomerGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api")
            .WithTags("Gateway - Customer Service");

        var customersGroup = group.MapGroup("/customers");

        customersGroup.MapGet("/", async (
                [FromServices] ICustomerService customerService) =>
            {
                var result = await customerService.GetCustomersAsync();
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("GetAllCustomers")
            .Produces<List<string>>(StatusCodes.Status400BadRequest);

        customersGroup.MapGet("/{id:guid}", async (
                [FromServices] ICustomerService customerService,
                Guid id) =>
            {
                var result = await customerService.GetCustomerByIdAsync(id);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("GetCustomerById")
            .Produces<List<string>>(StatusCodes.Status404NotFound);

        customersGroup.MapPost("/", async (
                [FromServices] ICustomerService customerService,
                [FromBody] CreateCustomerDto createCustomerDto) =>
            {
                var result = await customerService.CreateCustomerAsync(createCustomerDto);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("CreateCustomer")
            .Produces<CustomerDto>(StatusCodes.Status201Created)
            .Produces<List<string>>(StatusCodes.Status400BadRequest);
    }
}