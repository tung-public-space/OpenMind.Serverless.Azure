using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Queries;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for retrieving an order by ID
/// HTTP Trigger: GET /api/orders/{id}
/// </summary>
public class GetOrder
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetOrder> _logger;

    public GetOrder(IMediator mediator, ILogger<GetOrder> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("GetOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id:guid}")] HttpRequest req,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetOrder function triggered for ID: {OrderId}", id);

        try
        {
            var query = new GetOrderByIdQuery { OrderId = id };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", id);
                return new NotFoundObjectResult(new { error = $"Order with ID '{id}' not found" });
            }

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", id);
            return new ObjectResult(new { error = "An error occurred while retrieving the order" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

