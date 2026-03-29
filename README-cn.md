AI API 管理器
一个统一的 API 网关，用于接入多家 AI 服务提供商（OpenAI、Anthropic、Gemini、Azure OpenAI、Ollama）。
项目结构
plaintext
AI.API.Manager/
├── AI.API.Manager.Domain/          # 领域模型与业务逻辑
├── AI.API.Manager.Application/     # 应用服务与用例
├── AI.API.Manager.Infrastructure/  # 数据访问、外部服务集成
├── AI.API.Manager.API/             # Web API 层
└── AI.API.Manager.Tests/           # 测试项目
技术栈
框架：ASP.NET Core 8+
数据库：Entity Framework Core + SQL Server
弹性能力：Polly（重试 / 熔断器）
日志：Serilog
对象映射：AutoMapper
数据验证：FluentValidation
API 文档：Swashbuckle
测试：xUnit、FluentAssertions、Moq
第一阶段实现状态
✅ 已完成
 解决方案与项目结构搭建
 领域实体（采用 TDD 方式开发）：
租户（Tenant）
AI 服务提供商（AIProvider）
API 密钥（ApiKey）
请求日志（RequestLog）
 Entity Framework Core 数据库上下文（DbContext）
 数据库迁移配置
 依赖注入配置
 健康检查端点
 Docker 与 docker-compose 配置
 测试覆盖率框架
📊 测试覆盖率
测试总数：64 个通过的测试用例
行覆盖率：32.8%（项目初期，领域层已完全覆盖）
分支覆盖率：17.9%
测试类型：所有领域实体的单元测试
快速开始
前置条件
.NET 8 SDK 或更高版本
SQL Server（LocalDB 或完整实例）
Docker Desktop（用于容器化部署）
本地开发
克隆代码并还原依赖
bash
运行
git clone <仓库地址>
cd AI.API.Manager
dotnet restore
更新连接字符串（修改 AI.API.Manager.API/appsettings.json）
json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AIAPIManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
应用数据库迁移
bash
运行
cd AI.API.Manager
dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.API
运行应用程序
bash
运行
cd AI.API.Manager.API
dotnet run
访问 API
Swagger 文档界面：https://localhost:5001/swagger
健康检查：https://localhost:5001/health
Docker 部署
通过 Docker Compose 构建并运行
bash
运行
docker-compose up --build
访问服务
API 地址：http://localhost:5000
SQL Server：localhost:1433（账号：sa，密码：YourStrong!Passw0rd）
开发流程
测试驱动开发（TDD）
本项目严格遵循 TDD 原则：
红（RED）：先编写失败的测试用例
绿（GREEN）：实现最少代码使测试通过
重构（IMPROVE）：在保持测试通过的前提下重构代码
覆盖率（COVERAGE）：维持 80% 以上的测试覆盖率
运行测试
bash
运行
# 运行所有测试
dotnet test

# 运行测试并收集覆盖率
dotnet test --collect:"XPlat Code Coverage"

# 生成覆盖率报告
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
代码质量
使用 dotnet format 进行代码格式化
启用可空引用类型（nullable reference types）
遵循整洁架构（Clean Architecture）原则
尽可能保持对象不可变
API 端点（第一阶段）
健康检查
GET /health - 应用程序健康状态
第二阶段待实现
租户管理 API
AI 服务提供商配置
API 密钥管理
请求日志与数据分析
安全考量
源码中无硬编码密钥
基于环境的配置管理
API 边界的输入验证
通过 EF Core 防止 SQL 注入
API 密钥存储加密
下一步计划（第二阶段）
仓储模式实现
基于 CQRS 的应用服务
API 控制器与 DTO 定义
认证与授权
限流与节流
集成测试
贡献指南
遵循 TDD 开发流程（先写测试）
维持 80% 以上的测试覆盖率
使用规范化的提交信息
按需更新文档
提交前运行所有测试
许可证
[添加合适的许可证信息]
项目状态：第一阶段已完成 ✅最后更新：2026-03-28下一阶段：应用层功能实现
