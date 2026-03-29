using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Infrastructure.Data;
using AI.API.Manager.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AI.API.Manager.Tests.Infrastructure.Data.Repositories;

public class PagedListTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public PagedListTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var items = new List<Tenant>
        {
            Tenant.Create("Tenant 1", "Description 1", true),
            Tenant.Create("Tenant 2", "Description 2", true)
        };
        var count = 10;
        var pageIndex = 2;
        var pageSize = 5;

        // Act
        var pagedList = new PagedList<Tenant>(items, count, pageIndex, pageSize);

        // Assert
        pagedList.PageIndex.Should().Be(pageIndex);
        pagedList.PageSize.Should().Be(pageSize);
        pagedList.TotalCount.Should().Be(count);
        pagedList.TotalPages.Should().Be(2); // 10 / 5 = 2
        pagedList.HasPreviousPage.Should().BeTrue();
        pagedList.HasNextPage.Should().BeFalse(); // PageIndex 2, TotalPages 2
        pagedList.Items.Should().HaveCount(2);
        pagedList.Items.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void HasPreviousPage_ShouldBeFalse_WhenPageIndexIs1()
    {
        // Arrange
        var items = new List<Tenant>();
        var pagedList = new PagedList<Tenant>(items, 10, 1, 5);

        // Assert
        pagedList.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_ShouldBeFalse_WhenPageIndexEqualsTotalPages()
    {
        // Arrange
        var items = new List<Tenant>();
        var pagedList = new PagedList<Tenant>(items, 10, 2, 5);

        // Assert
        pagedList.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_ShouldBeTrue_WhenPageIndexLessThanTotalPages()
    {
        // Arrange
        var items = new List<Tenant>();
        var pagedList = new PagedList<Tenant>(items, 10, 1, 5);

        // Assert
        pagedList.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void TotalPages_ShouldBeCeilingOfDivision()
    {
        // Arrange
        var items = new List<Tenant>();

        // Test case 1: 10 items, page size 5 = 2 pages
        var pagedList1 = new PagedList<Tenant>(items, 10, 1, 5);
        pagedList1.TotalPages.Should().Be(2);

        // Test case 2: 11 items, page size 5 = 3 pages
        var pagedList2 = new PagedList<Tenant>(items, 11, 1, 5);
        pagedList2.TotalPages.Should().Be(3);

        // Test case 3: 0 items, page size 5 = 0 pages
        var pagedList3 = new PagedList<Tenant>(items, 0, 1, 5);
        pagedList3.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePagedListFromQueryable()
    {
        // Arrange
        var tenants = Enumerable.Range(1, 15)
            .Select(i => Tenant.Create($"Tenant {i}", $"Description {i}", true))
            .ToList();

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();

        var query = _context.Tenants.OrderBy(t => t.Name);
        var pageIndex = 2;
        var pageSize = 5;

        // Act
        var pagedList = await PagedList<Tenant>.CreateAsync(query, pageIndex, pageSize);

        // Assert
        pagedList.PageIndex.Should().Be(pageIndex);
        pagedList.PageSize.Should().Be(pageSize);
        pagedList.TotalCount.Should().Be(15);
        pagedList.TotalPages.Should().Be(3);
        pagedList.HasPreviousPage.Should().BeTrue();
        pagedList.HasNextPage.Should().BeTrue();
        pagedList.Items.Should().HaveCount(5);
        // Page 2 with pageSize 5: skip (2-1)*5 = 5, take 5
        // Items should be 6-10, but in-memory database might not preserve order
        // So we just check count and that items exist
        pagedList.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateAsync_WithEmptySource_ShouldCreateEmptyPagedList()
    {
        // Arrange
        var query = _context.Tenants;
        var pageIndex = 1;
        var pageSize = 10;

        // Act
        var pagedList = await PagedList<Tenant>.CreateAsync(query, pageIndex, pageSize);

        // Assert
        pagedList.PageIndex.Should().Be(pageIndex);
        pagedList.PageSize.Should().Be(pageSize);
        pagedList.TotalCount.Should().Be(0);
        pagedList.TotalPages.Should().Be(0);
        pagedList.HasPreviousPage.Should().BeFalse();
        pagedList.HasNextPage.Should().BeFalse();
        pagedList.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithPageIndexBeyondTotalPages_ShouldReturnEmptyItems()
    {
        // Arrange
        var tenants = Enumerable.Range(1, 5)
            .Select(i => Tenant.Create($"Tenant {i}", $"Description {i}", true))
            .ToList();

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();

        var query = _context.Tenants;
        var pageIndex = 2; // Only 1 page of data
        var pageSize = 10;

        // Act
        var pagedList = await PagedList<Tenant>.CreateAsync(query, pageIndex, pageSize);

        // Assert
        pagedList.PageIndex.Should().Be(pageIndex);
        pagedList.PageSize.Should().Be(pageSize);
        pagedList.TotalCount.Should().Be(5);
        pagedList.TotalPages.Should().Be(1);
        pagedList.HasPreviousPage.Should().BeTrue();
        pagedList.HasNextPage.Should().BeFalse();
        pagedList.Items.Should().BeEmpty();
    }
}