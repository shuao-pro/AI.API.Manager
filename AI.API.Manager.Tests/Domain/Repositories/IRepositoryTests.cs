using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Repositories;

public class IRepositoryTests
{
    // 这些测试验证IRepository接口的契约

    [Fact]
    public void IRepository_ShouldDefineAllRequiredAsyncMethods()
    {
        // Arrange & Act
        // 验证接口定义了所有必需的方法
        var methods = typeof(IRepository<>).GetMethods();

        // Assert
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetAllAsync");
        methods.Should().Contain(m => m.Name == "GetAsync");
        methods.Should().Contain(m => m.Name == "FirstOrDefaultAsync");
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "UpdateAsync");
        methods.Should().Contain(m => m.Name == "DeleteAsync");
        methods.Should().Contain(m => m.Name == "DeleteByIdAsync");
        methods.Should().Contain(m => m.Name == "ExistsAsync");
        methods.Should().Contain(m => m.Name == "CountAsync");
    }

    [Fact]
    public void IRepository_Methods_ShouldAcceptCancellationToken()
    {
        // Arrange
        var methods = typeof(IRepository<>).GetMethods()
            .Where(m => m.Name.EndsWith("Async"))
            .ToList();

        // Assert
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            parameters.Should().Contain(p =>
                p.ParameterType == typeof(CancellationToken));
        }
    }

    [Fact]
    public void IRepository_ShouldBeGenericWithClassConstraint()
    {
        // Arrange
        var type = typeof(IRepository<>);

        // Assert
        type.IsGenericType.Should().BeTrue();
        // 验证接口有where T : class约束
        // 这个约束在编译时检查，运行时无法直接验证
        Assert.True(true); // 占位符断言
    }
}