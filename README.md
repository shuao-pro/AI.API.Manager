# AI API Manager

A unified API gateway for accessing multiple AI service providers (OpenAI, Anthropic, Gemini, Azure OpenAI, Ollama).

## Project Structure

```
AI.API.Manager/
├── AI.API.Manager.Domain/          # Domain models and business logic
├── AI.API.Manager.Application/     # Application services and use cases
├── AI.API.Manager.Infrastructure/  # Data access, external services
├── AI.API.Manager.API/             # Web API layer
└── AI.API.Manager.Tests/           # Test projects
```

## Technology Stack

- **Framework**: ASP.NET Core 8+
- **Database**: Entity Framework Core + SQL Server
- **Resilience**: Polly (retry/circuit breaker)
- **Logging**: Serilog
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **API Documentation**: Swashbuckle
- **Testing**: xUnit, FluentAssertions, Moq

## Phase 1 Implementation Status

### ✅ Completed
- [x] Solution and project structure created
- [x] Domain entities with TDD approach:
  - Tenant
  - AIProvider
  - ApiKey
  - RequestLog
- [x] Entity Framework Core DbContext
- [x] Database migrations
- [x] Dependency injection configuration
- [x] Health check endpoints
- [x] Docker and docker-compose configuration
- [x] Test coverage framework

### 📊 Test Coverage
- **Total Tests**: 64 passing tests
- **Line Coverage**: 32.8% (early phase, Domain layer fully covered)
- **Branch Coverage**: 17.9%
- **Test Types**: Unit tests for all domain entities

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- SQL Server (LocalDB or full instance)
- Docker Desktop (for containerized deployment)

### Local Development

1. **Clone and restore**
   ```bash
   git clone <repository-url>
   cd AI.API.Manager
   dotnet restore
   ```

2. **Update connection string** in `AI.API.Manager.API/appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Apply database migrations**
   ```bash
   cd AI.API.Manager
   dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.API
   ```

4. **Run the application**
   ```bash
   cd AI.API.Manager.API
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: https://localhost:5001/swagger
   - Health check: https://localhost:5001/health

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Access the services**
   - API: http://localhost:5000
   - SQL Server: localhost:1433 (sa/YourStrong!Passw0rd)

## Development Workflow

### Test-Driven Development
This project follows strict TDD principles:
1. **RED**: Write failing tests first
2. **GREEN**: Implement minimal code to pass tests
3. **IMPROVE**: Refactor while keeping tests green
4. **COVERAGE**: Maintain 80%+ test coverage

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

### Code Quality
- Use `dotnet format` for code formatting
- Enable nullable reference types
- Follow Clean Architecture principles
- Maintain immutability where possible

## API Endpoints (Phase 1)

### Health Check
- `GET /health` - Application health status

### Coming in Phase 2
- Tenant management API
- AI provider configuration
- API key management
- Request logging and analytics

## Security Considerations

- No hardcoded secrets in source code
- Environment-based configuration
- Input validation at API boundaries
- SQL injection prevention via EF Core
- API key encryption at rest

## Next Steps (Phase 2)

1. **Repository pattern implementation**
2. **Application services with CQRS**
3. **API controllers and DTOs**
4. **Authentication and authorization**
5. **Rate limiting and throttling**
6. **Integration tests**

## Contributing

1. Follow TDD workflow (tests first)
2. Maintain 80%+ test coverage
3. Use conventional commit messages
4. Update documentation as needed
5. Run all tests before submitting

## License

[Add appropriate license]

---

**Project Status**: Phase 1 Complete ✅
**Last Updated**: 2026-03-28
**Next Phase**: Application layer implementation