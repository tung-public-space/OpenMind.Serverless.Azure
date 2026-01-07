using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Commands;
using Order.Application.Exceptions;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for cancelling an order
/// HTTP Trigger: DELETE /api/orders/{id}
/// </summary>
public class CancelOrder(IMediator mediator, ILogger<CancelOrder> logger)
{
    [Function("CancelOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "orders/{id:guid}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("CancelOrder function triggered for ID: {OrderId}", id);

        try
        {
            var reason = req.Query["reason"].ToString();
            if (string.IsNullOrWhiteSpace(reason))
            {
                reason = "Cancelled via API";
            }

            var command = new CancelOrderCommand
            {
                OrderId = id,
                Reason = reason
            };

            await mediator.Send(command, cancellationToken);

            logger.LogInformation("Order cancelled: {OrderId}", id);

            return new NoContentResult();
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
            logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return new ObjectResult(new { error = "An error occurred while cancelling the order" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

