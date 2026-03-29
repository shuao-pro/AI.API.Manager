# AI API Manager

A unified API gateway for accessing multiple AI service providers (OpenAI, Anthropic, Gemini, Azure OpenAI, Ollama).一个统一的API网关，用于访问多个AI服务提供商（OpenAI, Anthropic, Gemini, Azure OpenAI, Ollama）。

## Project Structure   ##项目结构

```
AI.API.Manager/
├── AI.API.Manager.Domain/          # Domain models and business logic├──AI.API.Manager。Domain/ #领域模型和业务逻辑
├── AI.API.Manager.Application/     # Application services and use cases├──AI.API.Manager。Application/ #应用程序服务和用例
├── AI.API.Manager.Infrastructure/  # Data access, external services├──AI.API.Manager。基础设施/ #数据访问，外部服务
├── AI.API.Manager.API/             # Web API layer
└── AI.API.Manager.Tests/           # Test projects└──AI.API.Manager。Tests/ #测试项目
```

## Technology Stack   ##技术栈

- **Framework**: ASP.NET Core 8+- **框架**:ASP。净核心8
- **Database**: Entity Framework Core + SQL Server- **数据库**:Entity Framework Core SQL Server- **数据库**:Entity Framework Core SQL Server- **数据库**:Entity Framework Core SQL Server- **数据库**:Entity Framework Core SQL Server—**数据库**:Entity Framework Core SQL Server—**数据库**:Entity Framework Core SQL Server—**数据库**:Entity Framework Core SQL Server
- **Resilience**: Polly (retry/circuit breaker)- **弹性**:Polly（重试/断路器）
- **Logging   日志记录**: Serilog
- **Mapping**: AutoMapper
- **Validation**: FluentValidation
- **API Documentation**: Swashbuckle- **API文档**:Swashbuckle
- **Testing**: xUnit, FluentAssertions, Moq- **测试**:xUnit、FluentAssertions、Moq

## Phase 1 Implementation Status##第一阶段的实现状态

### ✅ Completed   ###✅完成
- [x] Solution and project structure created- [x]创建解决方案和项目结构
- [x] Domain entities with TDD approach:- [x]使用TDD方法的领域实体：
  - Tenant   ——租户
  - AIProvider   ——AIProvider
  - ApiKey   ——ApiKey
  - RequestLog
- [x] Entity Framework Core DbContext- [x]实体框架核心DbContext
- [x] Database migrations   - [x]数据库迁移
- [x] Dependency injection configuration- [x]依赖注入配置
- [x] Health check endpoints- [x]健康检查端点
- [x] Docker and docker-compose configuration- [x] Docker和Docker -compose配置
- [x] Test coverage framework- [x]测试覆盖框架

### 📊 Test Coverage   ###📊测试覆盖率
- **Total Tests**: 64 passing tests- **测试总数**:64个测试通过
- **Line Coverage**: 32.8% (early phase, Domain layer fully covered)- **线路覆盖率**:32.8%（早期阶段，域层全覆盖）
- **Branch Coverage**: 17.9%- **分支覆盖率**:17.9%
- **Test Types**: Unit tests for all domain entities- **测试类型**：对所有域实体进行单元测试

## Getting Started   ##入门

### Prerequisites   # # #先决条件
- .NET 8 SDK or later-。NET 8 SDK或更高版本
- SQL Server (LocalDB or full instance)- SQL Server （LocalDB或全实例）
- Docker Desktop (for containerized deployment)- Docker桌面（用于容器化部署）

### Local Development   ###本地开发

1. **Clone and restore**   1. **克隆和还原**
   ```bash   ”“bash   “bash”;“bashBash & quot; Bash & quot; Bash & quot；```bash   ”“bash   “bash”;“bash
   git clone https://github.com/shuao-pro/AI.API.Manager.git
   cd AI.API.Manager
   dotnet restore
  ```

2. **Update connection string   更新连接字符串** in `AI.API.Manager.API/appsettings.json`2. **在`AI.API.Manager.API/appsettings.json`中更新连接字符串**
   ```json   ' ' ' json```json ```json```json ``json ``json ``json ``' ' ' json ' ' ' json ' json的json ' ' ' ' json的json的json的' json ' '' ' ' json ' ' ' json ' json的json ' ' ' ' json的json的json的json”“json ' json的json的json json的json的json的' ' ' ' ' json ' '
   "ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {"ConnectionStrings": {   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true""DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Apply database migrations**3. **应用数据库迁移**
   ```bash   ”“bash   “bash”;“bash```bash   ”“bash   “bash”;“bash
   cd AI.API.Manager
   dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.APIdotnet ef数据库更新——项目AI.API.Manager.Infrastructure——启动——项目AI.API.Manager.API
   ```

4. **Run the application**4. **运行应用程序**
   ```bash   ”“bash   “bash”;“bashBash & quot; Bash & quot; Bash & quot；```bash   ”“bash   “bash”;“bash
   cd AI.API.Manager.API
   dotnet run
   ```

5. **Access the API**   5. **访问API**
   - Swagger UI: https://localhost:5001/swagger
   - Health check: https://localhost:5001/health—健康检查：https://localhost:5001/health

### Docker Deployment   ### Docker部署

1. **Build and run with Docker Compose使用Docker Compose构建并运行**1. **使用Docker Compose构建并运行**
   ```bash   ”“bash   “bash”;“bash```bash   ”“bash   “bash”;“bash
   docker-compose up --buildDocker-compose up -build
   ```

2. **Access the services**
   - API: http://localhost:5000
   - SQL Server: localhost:1433 (sa/YourStrong!Passw0rd)

## Development Workflow

### Test-Driven Development
This project follows strict TDD principles:这个项目严格遵循TDD原则：
1. **RED**: Write failing tests first
2. **GREEN**: Implement minimal code to pass tests
3. **IMPROVE**: Refactor while keeping tests green
4. **COVERAGE**: Maintain 80%+ test coverage

### Running Tests
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

[MIT]

---

**Project Status**: Phase 1 Complete ✅
**Last Updated**: 2026-03-28
**Next Phase**: Application layer implementation
