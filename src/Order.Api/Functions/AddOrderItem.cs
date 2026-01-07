using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Commands;
using Order.Application.Exceptions;
using System.Text.Json;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for adding an item to an existing order
/// HTTP Trigger: POST /api/orders/{id}/items
/// </summary>
public class AddOrderItem(IMediator mediator, ILogger<AddOrderItem> logger)
{
    [Function("AddOrderItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders/{id:guid}/items")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("AddOrderItem function triggered for order: {OrderId}", id);

        try
        {
            var requestBody = await JsonSerializer.DeserializeAsync<AddItemRequest>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (requestBody == null)
            {
                return new BadRequestObjectResult(new { error = "Invalid request body" });
            }

            var command = new AddOrderItemCommand
            {
                OrderId = id,
                ProductId = requestBody.ProductId,
                ProductName = requestBody.ProductName,
                Quantity = requestBody.Quantity,
                UnitPrice = requestBody.UnitPrice,
                Currency = requestBody.Currency ?? "USD"
            };

            var result = await mediator.Send(command, cancellationToken);

            logger.LogInformation("Item added to order: {OrderId}", id);

            return new OkObjectResult(result);
        }
        catch (OrderNotFoundException ex)
        {
            logger.LogWarning("Order not found: {OrderId}", ex.OrderId);
            return new NotFoundObjectResult(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Invalid operation: {Message}", ex.Message);
            return new BadRequestObjectResult(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding item to order {OrderId}", id);
            return new ObjectResult(new { error = "An error occurred while adding the item" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    private record AddItemRequest(
        string ProductId, 
        string ProductName, 
        int Quantity, 
        decimal UnitPrice, 
        string? Currency);
}

