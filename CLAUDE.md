# FitForge AI

AI-powered workout planning platform. ASP.NET Core 8.0 + React 18 + PostgreSQL.

## Tech Stack

- **Backend:** ASP.NET Core 8.0, EF Core 8.0, PostgreSQL 15, Redis
- **Frontend:** React 18, TypeScript, Vite, Tailwind CSS, React Query, Zustand
- **AI:** GLM 4.7 Flash API
- **Messaging:** RabbitMQ via MassTransit
- **Testing:** xUnit, Moq, Playwright

## Architecture

Modular monolith with domain modules. See `DOMAIN_GLOSSARY.md` for entity definitions.

```
src/
├── FitForge.Api/           # ASP.NET Core Web API
├── FitForge.Core/          # Domain layer (entities, interfaces)
├── FitForge.Infrastructure/ # Data access, external services
└── FitForge.Shared/        # Shared utilities (Result pattern)
```

## Key Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Start Docker services
docker-compose up -d

# Start API
dotnet run --project src/FitForge.Api

# Verify
curl http://localhost:5000/health
curl http://localhost:5000/swagger/index.html
```

## Development Rules

1. Follow Result pattern for error handling
2. Return `Result<T>` from all service methods
3. Use async/await for all I/O operations
4. Write unit tests for all service methods
5. Follow naming conventions in documentation

## Database

- PostgreSQL with EF Core migrations
- Snake_case for column names
- UUID primary keys
- Timestamps as TIMESTAMPTZ

## API Conventions

- RESTful endpoints
- Standard error format (see `API_SPEC.md`)
- Pagination: page, pageSize, sortBy, sortOrder
- Authentication: JWT with refresh tokens

## Important Files

- `DOMAIN_GLOSSARY.md` - Business terms
- `REQUIREMENTS.md` - Functional requirements
- `API_SPEC.md` - API documentation
- `REVIEW_FitForge_AI_Documentation.md` - Architecture review
