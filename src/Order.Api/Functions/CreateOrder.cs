using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Commands;
using Order.Application.DTOs;
using System.Text.Json;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for creating new orders
/// HTTP Trigger: POST /api/orders
/// </summary>
public class CreateOrder
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateOrder> _logger;

    public CreateOrder(IMediator mediator, ILogger<CreateOrder> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("CreateOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateOrder function triggered");

        try
        {
            var command = await JsonSerializer.DeserializeAsync<CreateOrderCommand>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (command == null)
            {
                return new BadRequestObjectResult(new { error = "Invalid request body" });
            }

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Order created successfully: {OrderId}", result.Id);

            return new CreatedResult($"/api/orders/{result.Id}", result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Message);
            return new BadRequestObjectResult(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return new ObjectResult(new { error = "An error occurred while creating the order" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

