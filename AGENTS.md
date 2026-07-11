# AGENTS.md

Repo-specific guidance for AI agents working in this codebase. Verified against current sources.

## Repository layout

Monorepo with two projects orchestrated by `docker-compose.yml`:

- `WebAPI/` — .NET 8 ASP.NET Core Web API (C#, `net8.0`, Nullable enabled, ImplicitUsings enabled)
  - Solution: `WebAPI/WebAPI.sln`
  - App: `WebAPI/WebAPI/WebAPI.csproj`
  - Tests: `WebAPI/WebAPI.Tests/WebAPI.Tests.csproj` (xUnit + Moq + EF Core InMemory)
- `WebReact/` — React 18 + TypeScript + Vite 5 frontend
- `docker-compose.yml` — `myapi` (WebAPI), `webreact` (nginx), `mysql:8.0`, `mongodb`

## Commands

### WebAPI (run from `Demo/`)

```bash
dotnet restore WebAPI/WebAPI.sln
dotnet build --configuration Release WebAPI/WebAPI.sln
dotnet test WebAPI/WebAPI.Tests/WebAPI.Tests.csproj --verbosity normal          # all tests
dotnet test WebAPI/WebAPI.Tests/WebAPI.Tests.csproj --filter "FullyQualifiedName~RestaurantServiceTests"  # single class
dotnet run --project WebAPI/WebAPI/WebAPI.csproj                                # dev server on http://localhost:5129 (Swagger at /swagger)
```

CI (`.github/workflows/webapi.yml`) runs: restore → build Release → test. Triggers only on `WebAPI/**` path changes.

### WebReact (run from `WebReact/`)

```bash
npm install
npm run dev        # Vite dev server
npm run build      # tsc -b && vite build  (typecheck fails the build)
npm run lint       # eslint --max-warnings 0  (ANY warning fails)
npm run preview    # preview production build
```

CI (`.github/workflows/webreact.yml`) runs: npm install → lint → build. Triggers only on `WebReact/**` path changes.

## Architecture gotchas

### Dual persistence — two DbContexts

The API uses **two** EF Core DbContexts backed by different databases. This is the most non-obvious structural fact:

- `ApplicationDbContext` (namespace `WebAPI.Data`) — **MySQL** via `Pomelo.EntityFrameworkCore.MySql`. Holds `Products` only.
- `RestaurantReservationDbContext` (namespace **`WebAPI.Services`**, not `Data`) — **MongoDB** via `MongoDB.EntityFrameworkCore`. Holds `Restaurants` and `Reservations`. String Ids are configured to serialize as native MongoDB ObjectIds.

Both are registered in `Program.cs` along with three scoped services: `IRestaurantService`, `IReservationService`, `IProductService` (interface + impl pairs in `WebAPI/Services/`).

### Auto-migration on startup

`Program.cs` calls `dbContext.Database.Migrate()` on startup for `ApplicationDbContext`, so EF migrations apply automatically. The `WebAPI/Dockerfile` ENTRYPOINT is `sh -c "dotnet WebAPI.dll && dotnet ef database update"` — note the app starts before the `ef` call, and Program.cs `Migrate()` is the reliable migration path.

### Frontend routing & API proxy

- WebReact uses **`wouter`**, not `react-router`. Routing API differs (see `src/App.tsx`).
- HTTP via **`axios`**.
- `nginx.conf` proxies `/api/` and `/swagger/` to `http://myapi:8080` (the compose service name) — **only in the containerized deploy**. In Vite dev there is no proxy; talk to the API directly at `http://localhost:5129`.

## Linting & typecheck quirks (WebReact)

- ESLint runs with `--max-warnings 0` — any warning fails CI and local lint.
- `@typescript-eslint/no-explicit-any` is explicitly **OFF**.
- `tsconfig.app.json`: `strict`, `noUnusedLocals`, `noUnusedParameters`, `noFallthroughCasesInSwitch` — unused vars fail `tsc -b` and thus `npm run build`.
- `eslint-plugin-react-hooks` exhaustive-deps requires **full object references** in useEffect arrays, e.g. `[params]` not `[params?.id]`.

## Testing conventions (WebAPI)

- Stack: xUnit + Moq + `Microsoft.EntityFrameworkCore.InMemory` (for service tests that need a DbContext).
- Test project mirrors source layout: `WebAPI.Tests/Services/`, `Controllers/`, `Dto/`.
- Mock async service methods with `.Returns(Task.FromResult(entity))` where `entity` matches the method's return type — **not** `Task.CompletedTask`. This matters because `Add*Async` methods return the created entity (see convention below).

## Repo conventions

- **Pagination**: collection-returning service methods take `skip`/`take` (EF-style `Take`/`Skip`) and return `PagedResult<T>` (`WebAPI/Models/PagedResult.cs`: `Items`, `TotalCount`, `PageSize`, `CurrentPage`). `PaginationParamsDto` (`WebAPI/Dto/`) defaults `PageNumber=1`, `PageSize=10`. All `GetAll*`-style methods must support paging — none should return unbounded collections.
- **POST/return values**: service `Add*Async` methods return the created entity; controllers build response DTOs from the returned entity, not from the request input.
- **Commits**: gitmoji-style messages, e.g. `:bug: fix: message`.
- **Git safety**: never run `git reset --hard` or other destructive git operations without explicit user approval.

## Active refactoring context

`.omo/plans/webapi-recovery-*.md` tracks a 4-phase sequence (Core → Services → Controllers → Tests) introducing the pagination and POST-return-value conventions above. If picking up that work, read `webapi-recovery-master.md` first for the dependency matrix. Notepad state lives in `.omo/notepads/`.
