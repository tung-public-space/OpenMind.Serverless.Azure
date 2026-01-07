using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Queries;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for retrieving orders by customer ID
/// HTTP Trigger: GET /api/customers/{customerId}/orders
/// </summary>
public class GetOrdersByCustomer
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetOrdersByCustomer> _logger;

    public GetOrdersByCustomer(IMediator mediator, ILogger<GetOrdersByCustomer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("GetOrdersByCustomer")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/{customerId}/orders")] HttpRequest req,
        string customerId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetOrdersByCustomer function triggered for customer: {CustomerId}", customerId);

        try
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return new BadRequestObjectResult(new { error = "Customer ID is required" });
            }

            var query = new GetOrdersByCustomerQuery { CustomerId = customerId };
            var result = await _mediator.Send(query, cancellationToken);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for customer {CustomerId}", customerId);
            return new ObjectResult(new { error = "An error occurred while retrieving orders" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

