# Demo

Monorepo containing a .NET 8 Web API and a React 18 frontend, orchestrated with Docker Compose.

## Projects

| Project | Stack | Description |
|---------|-------|-------------|
| **WebAPI** | .NET 8 ASP.NET Core | REST API with dual database persistence |
| **WebReact** | React 18 + TypeScript + Vite + Mantine v7 | SPA frontend |

## Architecture

```
Demo/
├── WebAPI/          # .NET 8 Web API (port 5129)
├── WebReact/        # React SPA (port 5173 dev, served via nginx in prod)
├── docker-compose.yml
└── AGENTS.md        # AI agent guidance
```

### Resources

The API exposes three resource domains:

- **Products** — stored in MySQL via `ApplicationDbContext`
- **Restaurants** — stored in MongoDB via `RestaurantReservationDbContext`
- **Reservations** — stored in MongoDB via `RestaurantReservationDbContext`

### Dual Persistence

The API uses two separate EF Core `DbContext`s:

- `ApplicationDbContext` → **MySQL** (Pomelo) — holds `Products`
- `RestaurantReservationDbContext` → **MongoDB** — holds `Restaurants` and `Reservations`

See `AGENTS.md` for full architectural details.

## Quick Start

### Docker Compose (full stack)

```bash
docker compose up --build
```

- WebAPI: http://localhost:8080 → `myapi:8080` (Swagger at `/swagger`)
- WebReact: http://localhost:80 (nginx)

### WebAPI (from `Demo/`)

```bash
dotnet restore WebAPI/WebAPI.sln
dotnet build --configuration Release WebAPI/WebAPI.sln
dotnet test WebAPI/WebAPI.Tests/WebAPI.Tests.csproj --verbosity normal
dotnet run --project WebAPI/WebAPI/WebAPI.csproj
# → http://localhost:5129/swagger
```

### WebReact (from `WebReact/`)

```bash
npm install
npm run dev    # Vite dev server → http://localhost:5173
npm run build  # typecheck + production build
npm run lint   # ESLint (any warning fails)
npm run preview
```

> **Note:** In dev mode the SPA talks directly to `http://localhost:5129`. In the containerized setup, nginx proxies `/api` and `/swagger` to the `myapi` service.
