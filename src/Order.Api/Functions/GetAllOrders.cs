using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Order.Application.Queries;

namespace Order.Api.Functions;

/// <summary>
/// Azure Function for retrieving all orders with pagination
/// HTTP Trigger: GET /api/orders?skip=0&take=100
/// </summary>
public class GetAllOrders
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllOrders> _logger;

    public GetAllOrders(IMediator mediator, ILogger<GetAllOrders> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("GetAllOrders")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetAllOrders function triggered");

        try
        {
            var skip = int.TryParse(req.Query["skip"], out var s) ? s : 0;
            var take = int.TryParse(req.Query["take"], out var t) ? t : 100;

            var query = new GetAllOrdersQuery { Skip = skip, Take = take };
            var result = await _mediator.Send(query, cancellationToken);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return new ObjectResult(new { error = "An error occurred while retrieving orders" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}

