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
/// Azure Function for updating order status
/// HTTP Trigger: PATCH /api/orders/{id}/status
/// </summary>
public class UpdateOrderStatus
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateOrderStatus> _logger;

    public UpdateOrderStatus(IMediator mediator, ILogger<UpdateOrderStatus> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("UpdateOrderStatus")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "orders/{id:guid}/status")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateOrderStatus function triggered for ID: {OrderId}", id);

        try
        {
            var requestBody = await JsonSerializer.DeserializeAsync<UpdateStatusRequest>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (requestBody == null || string.IsNullOrWhiteSpace(requestBody.Action))
            {
                return new BadRequestObjectResult(new { error = "Action is required (confirm, ship, deliver, cancel)" });
            }

            var command = new UpdateOrderStatusCommand
            {
                OrderId = id,
                Action = requestBody.Action,
                Reason = requestBody.Reason
            };

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Order status updated: {OrderId} -> {Action}", id, requestBody.Action);

            return new OkObjectResult(result);
        }
        catch (OrderNotFoundException ex)
        {
            _logger.LogWarning("Order not found: {OrderId}", ex.OrderId);
            return new NotFoundObjectResult(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation: {Message}", ex.Message);
            return new BadRequestObjectResult(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status {OrderId}", id);
            return new ObjectResult(new { error = "An error occurred while updating the order status" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    private record UpdateStatusRequest(string Action, string? Reason);
}

