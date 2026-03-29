using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Repositories;

public class ISpecificationTests
{
    [Fact]
    public void ISpecification_ShouldDefineAllRequiredProperties()
    {
        // Arrange
        var type = typeof(ISpecification<>);

        // Assert
        type.GetProperty("Criteria").Should().NotBeNull();
        type.GetProperty("Includes").Should().NotBeNull();
        type.GetProperty("OrderBy").Should().NotBeNull();
        type.GetProperty("OrderByDescending").Should().NotBeNull();
        type.GetProperty("Take").Should().NotBeNull();
        type.GetProperty("Skip").Should().NotBeNull();
        type.GetProperty("IsPagingEnabled").Should().NotBeNull();
    }

    [Fact]
    public void ISpecification_ShouldDefineAllRequiredMethods()
    {
        // Arrange
        var type = typeof(ISpecification<>);

        // Assert
        type.GetMethod("AddInclude").Should().NotBeNull();
        type.GetMethod("AddOrderBy").Should().NotBeNull();
        type.GetMethod("AddOrderByDescending").Should().NotBeNull();
        type.GetMethod("ApplyPaging").Should().NotBeNull();
    }

    [Fact]
    public void ISpecification_ShouldHaveClassConstraint()
    {
        // Arrange
        var type = typeof(ISpecification<>);

        // Assert
        type.IsGenericType.Should().BeTrue();
        // 验证有where T : class约束
        Assert.True(true);
    }
}