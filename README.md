# Order Service - Serverless Implementation

A serverless Order Service built with Azure Functions (.NET 9) following Domain-Driven Design (DDD) principles and Clean Architecture.

## 📁 Project Structure

```
order-service/
├── src/
│   ├── Order.Api/                # Serverless function app (Azure Functions)
│   │   ├── Functions/
│   │   │   ├── CreateOrder.cs
│   │   │   ├── GetOrder.cs
│   │   │   ├── GetAllOrders.cs
│   │   │   ├── GetOrdersByCustomer.cs
│   │   │   ├── UpdateOrderStatus.cs
│   │   │   ├── CancelOrder.cs
│   │   │   └── AddOrderItem.cs
│   │   ├── Program.cs
│   │   └── Order.Api.csproj
│   │
│   ├── Order.Application/        # Use cases, commands, queries (CQRS)
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Mappers/
│   │
│   ├── Order.Domain/             # Entities, aggregates, value objects
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Events/
│   │   └── Repositories/
│   │
│   └── Order.Infrastructure/     # Database, EventBus, external services
│       ├── Repositories/
│       └── EventBus/
│
├── tests/
│   ├── Order.UnitTests/
│   └── Order.IntegrationTests/
│
├── deploy/
│   └── azure/
│       ├── main.bicep
│       ├── parameters.dev.json
│       └── parameters.prod.json
│
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (for deployment)
- [Azurite](https://docs.microsoft.com/azure/storage/common/storage-use-azurite) (local storage emulator)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd order-service
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Start Azurite (in a separate terminal)**
   ```bash
   azurite --silent --location ./azurite --debug ./azurite/debug.log
   ```

6. **Run the Function App locally**
   ```bash
   cd src/Order.Api
   func start
   ```

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create a new order |
| GET | `/api/orders` | Get all orders (with pagination) |
| GET | `/api/orders/{id}` | Get order by ID |
| GET | `/api/customers/{customerId}/orders` | Get orders by customer |
| PATCH | `/api/orders/{id}/status` | Update order status |
| POST | `/api/orders/{id}/items` | Add item to order |
| DELETE | `/api/orders/{id}` | Cancel order |

### Example Requests

**Create Order**
```json
POST /api/orders
{
  "customerId": "customer-123",
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Seattle",
    "state": "WA",
    "postalCode": "98101",
    "country": "USA"
  },
  "items": [
    {
      "productId": "product-1",
      "productName": "Widget",
      "quantity": 2,
      "unitPrice": 29.99,
      "currency": "USD"
    }
  ]
}
```

**Update Order Status**
```json
PATCH /api/orders/{id}/status
{
  "action": "confirm"  // confirm, ship, deliver, cancel
}
```

## 🏗️ Architecture

### Clean Architecture Layers

1. **Domain Layer** (`Order.Domain`)
   - Contains business entities, value objects, and domain events
   - No dependencies on other layers
   - Pure business logic

2. **Application Layer** (`Order.Application`)
   - Contains use cases (commands and queries)
   - Uses MediatR for CQRS pattern
   - FluentValidation for input validation

3. **Infrastructure Layer** (`Order.Infrastructure`)
   - Contains implementations of repositories and external services
   - Currently uses in-memory storage (swap for CosmosDB, SQL, etc.)

4. **API Layer** (`Order.Api`)
   - Azure Functions HTTP triggers
   - Entry points for the application
   - Each function is independently deployable

### Domain Events

The service publishes domain events:
- `OrderCreatedEvent`
- `OrderConfirmedEvent`
- `OrderShippedEvent`
- `OrderDeliveredEvent`
- `OrderCancelledEvent`
- `OrderItemAddedEvent`
- `OrderItemRemovedEvent`

## 🧪 Testing

### Unit Tests
```bash
dotnet test tests/Order.UnitTests
```

### Integration Tests
```bash
dotnet test tests/Order.IntegrationTests
```

## 🚢 Deployment

### Using Azure CLI

1. **Create resource group**
   ```bash
   az group create --name order-service-rg --location eastus
   ```

2. **Deploy infrastructure**
   ```bash
   az deployment group create \
     --resource-group order-service-rg \
     --template-file deploy/azure/main.bicep \
     --parameters @deploy/azure/parameters.dev.json
   ```

3. **Deploy Function App**
   ```bash
   cd src/Order.Api
   func azure functionapp publish <function-app-name>
   ```

### Using GitHub Actions

The CI/CD pipeline is configured in `.github/workflows/order-service-ci-cd.yml`:
- Builds and tests on every PR
- Deploys to dev environment on `develop` branch
- Deploys to production on `main` branch

Required secrets:
- `AZURE_CREDENTIALS`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_RESOURCE_GROUP_DEV`
- `AZURE_RESOURCE_GROUP_PROD`
- `AZURE_FUNCTIONAPP_NAME_DEV`
- `AZURE_FUNCTIONAPP_NAME_PROD`

## 📝 Best Practices Applied

✅ **One repo per service (bounded context)**
✅ **Multiple functions in one project**
✅ **Shared domain logic across functions**
✅ **DDD-friendly structure**
✅ **CQRS pattern with MediatR**
✅ **Clean Architecture separation**
✅ **Single CI/CD pipeline**
✅ **Infrastructure as Code (Bicep)**

## 🔄 Extending the Service

### Adding a new function

1. Create a new file in `src/Order.Api/Functions/`
2. Add corresponding command/query in `Order.Application`
3. Update domain if needed in `Order.Domain`

### Replacing in-memory storage

1. Add database package (e.g., `Microsoft.Azure.Cosmos`)
2. Implement `IOrderRepository` with actual database
3. Update `DependencyInjection.cs` in Infrastructure layer

## 📚 Related Services

This Order Service is designed to work with:
- **Payment Service** - Handles payment processing
- **Inventory Service** - Manages stock levels
- **Notification Service** - Sends order notifications

## 📄 License

MIT License

