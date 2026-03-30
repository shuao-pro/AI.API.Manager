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

4. **Run the application**4. **运行应用程序**
   ```bash
   cd AI.API.Manager.API
   dotnet run
   ```

5. **Access the API**   5. **访问API**
   - Swagger UI: https://localhost:5001/swagger
   - Health check: https://localhost:5001/health—健康检查：https://localhost:5001/health

### Docker Deployment   ### Docker部署

1. **Build and run with Docker Compose**1. **使用Docker Compose构建并运行**
   ```bash   ”“bash
   docker-compose up --buildDocker-compose up -build
   ```

2. **Access the services**2. **访问服务**
   - API: http://localhost:5000
   - SQL Server: localhost:1433 (sa/YourStrong!Passw0rd)—SQL Server: localhost:1433 (sa/YourStrong！Passw0rd)

## Development Workflow   ##开发流程

### Test-Driven Development###测试驱动开发
This project follows strict TDD principles:这个项目严格遵循TDD原则：
1. **RED**: Write failing tests first1. **红色**：首先编写失败的测试
2. **GREEN**: Implement minimal code to pass tests2. **绿色**：实现最少的代码以通过测试
3. **IMPROVE**: Refactor while keeping tests green3. **改进**：重构的同时保持测试绿色
4. **COVERAGE**: Maintain 80%+ test coverage4. **覆盖率**：保持80%的测试覆盖率

### Running Tests   ###运行测试
```bash   ”“bash
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

##3. **改进**：重构的同时保持测试绿色 Next Steps (Phase 2)   ##下一步（第二阶段）

1. **Repository pattern implementation**1. **Repository模式的实现**
2. **Application services with CQRS**2. **使用CQRS的应用服务**
3. **API controllers and DTOs**3. **API控制器和dto **
4. **Authentication and authorization**4. **身份验证和授权**
5. **Rate limiting and throttling**5. **速率限制和节流**
6. **Integration tests**   6. * * * *集成测试

## Contributing   # #贡献

1. Follow TDD workflow (tests first)1. 遵循TDD工作流程（先测试）
2. Maintain 80%+ test coverage2. 保持80%的测试覆盖率
3. Use conventional commit messages3. 使用常规的提交消息
4. Update documentation as needed4. 根据需要更新文档
5. Run all tests before submitting5. 在提交之前运行所有测试

## License   # #许可证

[MIT]

---

**Project Status**: Phase 1 Complete ✅**项目状态**：第一阶段完成✅
**Last Updated**: 2026-03-28**最后更新**:2026-03-28
**Next Phase**: Application layer implementation**下一阶段**：应用层实现
