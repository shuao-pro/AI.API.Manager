using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Repositories;

public class IPagedListTests
{
    [Fact]
    public void IPagedList_ShouldDefineAllRequiredProperties()
    {
        // Arrange
        var type = typeof(IPagedList<>);

        // Assert
        type.GetProperty("PageIndex").Should().NotBeNull();
        type.GetProperty("PageSize").Should().NotBeNull();
        type.GetProperty("TotalCount").Should().NotBeNull();
        type.GetProperty("TotalPages").Should().NotBeNull();
        type.GetProperty("HasPreviousPage").Should().NotBeNull();
        type.GetProperty("HasNextPage").Should().NotBeNull();
        type.GetProperty("Items").Should().NotBeNull();
    }

    [Fact]
    public void IPagedList_Properties_ShouldHaveCorrectTypes()
    {
        // Arrange
        var type = typeof(IPagedList<>);

        // Assert
        type.GetProperty("PageIndex")!.PropertyType.Should().Be(typeof(int));
        type.GetProperty("PageSize")!.PropertyType.Should().Be(typeof(int));
        type.GetProperty("TotalCount")!.PropertyType.Should().Be(typeof(int));
        type.GetProperty("TotalPages")!.PropertyType.Should().Be(typeof(int));
        type.GetProperty("HasPreviousPage")!.PropertyType.Should().Be(typeof(bool));
        type.GetProperty("HasNextPage")!.PropertyType.Should().Be(typeof(bool));
        type.GetProperty("Items")!.PropertyType.Name.Should().Contain("IReadOnlyList");
    }
}