# AI.API.Manager - 统一AI服务API网关

## 🚀 项目简介

AI.API.Manager 是一个现代化的统一API网关，专为管理和集成多个AI服务提供商而设计。该项目采用Clean Architecture和测试驱动开发(TDD)方法构建，提供了一套完整的解决方案，帮助开发者轻松接入和管理OpenAI、Anthropic、Gemini、Azure OpenAI、Ollama等主流AI服务。

## ✨ 核心特性

### 🔌 多AI提供商支持
- **统一接口**：标准化不同AI服务的API调用
- **提供商管理**：支持动态添加和配置AI服务提供商
- **智能路由**：基于成本、性能和可用性的智能请求路由
- **故障转移**：自动故障检测和备用提供商切换

### 🛡️ 企业级功能
- **租户隔离**：多租户架构，确保数据安全和隔离
- **API密钥管理**：安全的API密钥存储和轮换机制
- **请求日志**：完整的请求审计和性能监控
- **速率限制**：基于租户和提供商的智能限流

### 📊 监控与分析
- **实时监控**：服务健康状态和性能指标
- **使用分析**：详细的用量统计和成本分析
- **错误追踪**：完整的错误日志和调试信息
- **报表导出**：支持多种格式的数据导出

## 🏗️ 技术架构

### 架构模式
- **Clean Architecture**：清晰的关注点分离
- **领域驱动设计(DDD)**：以业务为核心的建模
- **CQRS模式**：命令查询职责分离
- **事件驱动**：松耦合的组件通信

### 技术栈
- **后端框架**：ASP.NET Core 8+
- **数据库**：Entity Framework Core + SQL Server
- **容器化**：Docker + Docker Compose
- **测试框架**：xUnit + Moq + FluentAssertions
- **代码质量**：SonarQube + Coverlet

## 📁 项目结构

```
AI.API.Manager/
├── AI.API.Manager.Domain/          # 领域层 - 业务核心模型和规则
├── AI.API.Manager.Application/     # 应用层 - 用例和服务
├── AI.API.Manager.Infrastructure/  # 基础设施层 - 外部服务和数据访问
├── AI.API.Manager.API/             # 表现层 - Web API和控制器
└── AI.API.Manager.Tests/           # 测试项目 - 单元和集成测试
```

## 🚦 快速开始

### 环境要求
- .NET 8 SDK 或更高版本
- SQL Server (LocalDB 或完整实例)
- Docker Desktop (可选，用于容器化部署)

### 本地开发

1. **克隆项目**
   ```bash
   git clone https://github.com/yourusername/AI.API.Manager.git
   cd AI.API.Manager
   ```

2. **恢复依赖**
   ```bash
   dotnet restore
   ```

3. **配置数据库**
   - 更新 `AI.API.Manager.API/appsettings.json` 中的连接字符串
   - 运行数据库迁移：
     ```bash
     dotnet ef database update --project AI.API.Manager.Infrastructure --startup-project AI.API.Manager.API
     ```

4. **启动应用**
   ```bash
   cd AI.API.Manager.API
   dotnet run
   ```

5. **访问API**
   - Swagger UI: https://localhost:5001/swagger
   - 健康检查: https://localhost:5001/health

### Docker部署
```bash
# 构建并启动所有服务
docker-compose up --build

# 访问服务
# API: http://localhost:5000
# SQL Server: localhost:1433
```

## 🧪 测试策略

### 测试覆盖率
- **单元测试**：领域层100%覆盖
- **集成测试**：API端点和数据库操作
- **端到端测试**：完整业务流程验证

### 运行测试
```bash
# 运行所有测试
dotnet test

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"

# 查看覆盖率报告
open coverage-report/index.html
```

## 📈 开发路线图

### ✅ 已完成 (Phase 1)
- [x] 解决方案和项目结构
- [x] 领域实体设计 (TDD方法)
- [x] Entity Framework Core 配置
- [x] 数据库迁移
- [x] 依赖注入配置
- [x] 健康检查端点
- [x] Docker配置
- [x] 测试框架搭建

### 🔄 进行中 (Phase 2)
- [ ] 仓储模式实现
- [ ] 应用服务层开发
- [ ] API控制器和DTO
- [ ] 身份认证和授权
- [ ] 速率限制和流量控制
- [ ] 集成测试

### 📅 计划中 (Phase 3)
- [ ] 管理仪表板
- [ ] 实时监控
- [ ] 高级分析功能
- [ ] Webhook支持
- [ ] 插件系统

## 🔒 安全特性

- **API密钥加密**：AES-256加密存储
- **输入验证**：FluentValidation全面验证
- **SQL注入防护**：EF Core参数化查询
- **CORS配置**：严格的原点控制
- **HTTPS强制**：生产环境强制HTTPS
- **安全头**：现代Web安全头设置

## 🤝 贡献指南

我们欢迎所有形式的贡献！请遵循以下步骤：

1. **Fork项目**
2. **创建功能分支** (`git checkout -b feature/amazing-feature`)
3. **提交更改** (`git commit -m 'Add some amazing feature'`)
4. **推送到分支** (`git push origin feature/amazing-feature`)
5. **开启Pull Request**

### 开发规范
- 遵循TDD原则（测试先行）
- 保持80%以上的测试覆盖率
- 使用约定式提交消息
- 及时更新文档
- 提交前运行所有测试

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🌟 项目状态

**当前版本**: v0.1.0 (Alpha)  
**构建状态**: [![Build Status](https://github.com/yourusername/AI.API.Manager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/yourusername/AI.API.Manager/actions)  
**测试覆盖率**: [![Coverage](https://coveralls.io/repos/github/yourusername/AI.API.Manager/badge.svg)](https://coveralls.io/github/yourusername/AI.API.Manager)  
**最新更新**: 2026-03-30

---

**让AI集成变得简单而强大！** 🚀

*AI.API.Manager - 您的AI服务统一管理平台*

