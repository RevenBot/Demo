# WebAPI

.NET 8 ASP.NET Core Web API for the Demo monorepo.

## Stack

- **ASP.NET Core 8** with controllers
- **Entity Framework Core** with two DbContexts
- **MySQL** via `Pomelo.EntityFrameworkCore.MySql` — `ApplicationDbContext`
- **MongoDB** via `MongoDB.EntityFrameworkCore` — `RestaurantReservationDbContext`
- **xUnit + Moq + EF Core InMemory** for tests

## Dual Persistence

Two separate `DbContext` registrations, backed by different databases:

| DbContext | Database | Namespaces | Holds |
|-----------|----------|------------|-------|
| `ApplicationDbContext` | MySQL | `WebAPI.Data` | `Products` |
| `RestaurantReservationDbContext` | MongoDB | `WebAPI.Services` | `Restaurants`, `Reservations` |

String IDs are serialized as native MongoDB ObjectIds.

## Commands (run from `Demo/`)

```bash
dotnet restore WebAPI/WebAPI.sln
dotnet build --configuration Release WebAPI/WebAPI.sln
dotnet test WebAPI/WebAPI.Tests/WebAPI.Tests.csproj --verbosity normal  # all tests
dotnet test WebAPI/WebAPI.Tests/WebAPI.Tests.csproj --filter "FullyQualifiedName~RestaurantServiceTests"
dotnet run --project WebAPI/WebAPI/WebAPI.csproj
# → http://localhost:5129/swagger
```

## Resources

| Resource | DbContext | Database |
|----------|-----------|----------|
| `Products` | `ApplicationDbContext` | MySQL |
| `Restaurants` | `RestaurantReservationDbContext` | MongoDB |
| `Reservations` | `RestaurantReservationDbContext` | MongoDB |

## Conventions

- **Pagination**: collection-returning service methods take `skip`/`take` and return `PagedResult<T>`
- **POST return values**: `Add*Async` methods return the created entity; controllers build DTOs from the returned entity
- **Auto-migration**: `ApplicationDbContext` runs `Database.Migrate()` on startup

## Testing

Tests live in `WebAPI/WebAPI.Tests/` and mirror the source layout (`Services/`, `Controllers/`, `Dto/`). Async service methods are mocked with `.Returns(Task.FromResult(entity))` — not `Task.CompletedTask`.
