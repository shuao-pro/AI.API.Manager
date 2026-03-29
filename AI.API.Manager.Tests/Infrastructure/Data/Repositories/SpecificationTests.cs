using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Infrastructure.Data.Repositories;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;

namespace AI.API.Manager.Tests.Infrastructure.Data.Repositories;

public class SpecificationTests
{
    [Fact]
    public void Constructor_WithoutCriteria_ShouldCreateEmptySpecification()
    {
        // Act
        var spec = new Specification<Tenant>();

        // Assert
        spec.Criteria.Should().BeNull();
        spec.Includes.Should().BeEmpty();
        spec.OrderBy.Should().BeEmpty();
        spec.OrderByDescending.Should().BeEmpty();
        spec.Take.Should().Be(0);
        spec.Skip.Should().Be(0);
        spec.IsPagingEnabled.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithCriteria_ShouldSetCriteria()
    {
        // Arrange
        Expression<Func<Tenant, bool>> criteria = t => t.IsActive;

        // Act
        var spec = new Specification<Tenant>(criteria);

        // Assert
        spec.Criteria.Should().Be(criteria);
    }

    [Fact]
    public void AddInclude_ShouldAddIncludeExpression()
    {
        // Arrange
        var spec = new Specification<Tenant>();
        Expression<Func<Tenant, object>> include = t => t.Name;

        // Act
        spec.AddInclude(include);

        // Assert
        spec.Includes.Should().ContainSingle();
        spec.Includes.First().Should().Be(include);
    }

    [Fact]
    public void AddOrderBy_ShouldAddOrderByExpression()
    {
        // Arrange
        var spec = new Specification<Tenant>();
        Expression<Func<Tenant, object>> orderBy = t => t.CreatedAt;

        // Act
        spec.AddOrderBy(orderBy);

        // Assert
        spec.OrderBy.Should().ContainSingle();
        spec.OrderBy.First().Should().Be(orderBy);
    }

    [Fact]
    public void AddOrderByDescending_ShouldAddOrderByDescendingExpression()
    {
        // Arrange
        var spec = new Specification<Tenant>();
        Expression<Func<Tenant, object>> orderByDescending = t => t.UpdatedAt;

        // Act
        spec.AddOrderByDescending(orderByDescending);

        // Assert
        spec.OrderByDescending.Should().ContainSingle();
        spec.OrderByDescending.First().Should().Be(orderByDescending);
    }

    [Fact]
    public void ApplyPaging_ShouldSetPagingProperties()
    {
        // Arrange
        var spec = new Specification<Tenant>();

        // Act
        spec.ApplyPaging(10, 20);

        // Assert
        spec.Skip.Should().Be(10);
        spec.Take.Should().Be(20);
        spec.IsPagingEnabled.Should().BeTrue();
    }

    [Fact]
    public void ApplyPaging_WithZeroTake_ShouldEnablePaging()
    {
        // Arrange
        var spec = new Specification<Tenant>();

        // Act
        spec.ApplyPaging(0, 0);

        // Assert
        spec.IsPagingEnabled.Should().BeTrue();
    }

    [Fact]
    public void MultipleIncludes_ShouldBeAddedCorrectly()
    {
        // Arrange
        var spec = new Specification<Tenant>();
        Expression<Func<Tenant, object>> include1 = t => t.Name;
        Expression<Func<Tenant, object>> include2 = t => t.Description;

        // Act
        spec.AddInclude(include1);
        spec.AddInclude(include2);

        // Assert
        spec.Includes.Should().HaveCount(2);
        spec.Includes.Should().Contain(include1);
        spec.Includes.Should().Contain(include2);
    }

    [Fact]
    public void MultipleOrderBy_ShouldBeAddedCorrectly()
    {
        // Arrange
        var spec = new Specification<Tenant>();
        Expression<Func<Tenant, object>> orderBy1 = t => t.CreatedAt;
        Expression<Func<Tenant, object>> orderBy2 = t => t.Name;

        // Act
        spec.AddOrderBy(orderBy1);
        spec.AddOrderBy(orderBy2);

        // Assert
        spec.OrderBy.Should().HaveCount(2);
        spec.OrderBy.Should().Contain(orderBy1);
        spec.OrderBy.Should().Contain(orderBy2);
    }
}