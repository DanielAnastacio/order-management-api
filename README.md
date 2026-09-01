# OrderManagement API

RESTful backend for a simple e-commerce order management system, implemented as a Senior .NET technical assessment.

The solution prioritizes clean architecture, separation of concerns, testability, and maintainability.

## Tech Stack

- .NET 10 / ASP.NET Core
- Clean Architecture
- CQRS with MediatR
- FluentValidation
- Entity Framework Core
- SQLite
- JWT authentication
- xUnit + Moq
- Docker + Docker Compose

## Architecture

The solution is divided into four main layers:

- **Domain** — business entities, state and invariants.
- **Application** — use cases, commands, queries, handlers, validation and repository abstractions.
- **Infrastructure** — EF Core, SQLite, mappings, migrations and repository implementations.
- **API** — HTTP controllers, JWT authentication, dependency injection and global exception handling.

Dependencies point inward: the Domain does not depend on infrastructure or HTTP concerns.

### Why Controllers?

Controllers were chosen instead of Minimal APIs to keep the HTTP layer explicit and familiar in a team-oriented application. Controllers remain thin and delegate use cases to MediatR; business rules stay in the Domain/Application layers.

## Business Rules

- An order must contain at least one item.
- `Quantity` must be greater than zero.
- `UnitPrice` must be greater than zero.
- New orders start as `Pending`.
- Only `Pending` orders can be cancelled.
- `TotalAmount` is calculated in the Domain from `Quantity * UnitPrice`.

## Authentication

The assessment uses a fixed in-memory user:

- **Email:** `dev@martech.com`
- **Password:** `Senha@123`

Login:

```http
POST /auth/login
```

The returned JWT must be sent to protected endpoints:

```http
Authorization: Bearer <token>
```

> The JWT signing key is stored in `appsettings.json` only to keep this technical assessment self-contained. In production it should come from an environment variable or a secrets-management solution.

## Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/auth/login` | Returns a JWT |
| POST | `/api/orders` | Creates an order |
| GET | `/api/orders?page=1&pageSize=10` | Lists orders with pagination |
| GET | `/api/orders/{id}` | Gets an order by id |
| PATCH | `/api/orders/{id}/cancel` | Cancels a pending order |

All `/api/orders` endpoints require authentication.

Example create request:

```json
{
  "customerId": "8d73db0a-0cf1-4cf2-a170-89441dbe51b7",
  "items": [
    {
      "productName": "Notebook",
      "quantity": 1,
      "unitPrice": 5000.00
    },
    {
      "productName": "Mouse",
      "quantity": 2,
      "unitPrice": 150.00
    }
  ]
}
```

A successful cancellation returns `204 No Content`.

## Request Flow

```text
HTTP Request
    ↓
Controller
    ↓
MediatR
    ↓
ValidationBehavior
    ↓
Handler
    ↓
Domain
    ↓
IOrderRepository
    ↓
EF Core / SQLite
```

FluentValidation validates incoming use-case data before handlers execute. The Domain independently protects its business invariants.

## Error Handling

The API uses centralized exception handling:

- Validation errors → `400 Bad Request`
- Business-rule violations → `400 Bad Request`
- Resource not found → `404 Not Found`
- Authentication failure → `401 Unauthorized`
- Unexpected errors → `500 Internal Server Error`

## Database

SQLite is used through Entity Framework Core.

Migrations are applied automatically on startup with `MigrateAsync()`.

Main tables:

- `Orders`
- `OrderItems`
- `__EFMigrationsHistory`

`TotalAmount` is calculated by the Domain and is not persisted as a separate column.

## Run Locally

Requirements:

- .NET 10 SDK

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project OrderManagement.Api
```

Using the default launch settings, the HTTP endpoint is:

```text
http://localhost:5069
```

## Run Tests

```bash
dotnet test
```

The Application handlers are covered by unit tests using xUnit and Moq. Repository dependencies are mocked so these tests do not require EF Core, SQLite, Docker, or the HTTP API.

## Run with Docker Compose

Requirements:

- Docker
- Docker Compose

From the repository root:

```bash
docker compose up --build
```

The API will be available at:

```text
http://localhost:8082
```

Stop it with:

```bash
docker compose down
```

The Compose configuration uses a Docker volume for the SQLite data file.

## Postman

A Postman collection is available under:

```text
postman/OrderManagement.postman_collection.json
```

Recommended execution order:

1. Login
2. Create Order
3. List Orders
4. Get Order By Id
5. Cancel Order

The collection stores the JWT and the created Order ID automatically.

## Main Design Decisions

- Clean Architecture isolates business rules from technical concerns.
- CQRS separates write and read use cases.
- MediatR decouples HTTP controllers from application handlers.
- Domain entities encapsulate business invariants.
- FluentValidation runs through a MediatR pipeline behavior.
- A specific `IOrderRepository` is used instead of an unnecessary generic repository.
- EF Core mappings and migrations live in Infrastructure.
- Controllers were preferred for an explicit HTTP organization.
- SQLite keeps the assessment lightweight and easy to execute.

## Repository Structure

```text
OrderManagement/
├── OrderManagement.Api/
├── OrderManagement.Application/
├── OrderManagement.Domain/
├── OrderManagement.Infrastructure/
├── OrderManagement.Application.Tests/
├── postman/
├── docker-compose.yml
├── README.md
└── OrderManagement.slnx
```
