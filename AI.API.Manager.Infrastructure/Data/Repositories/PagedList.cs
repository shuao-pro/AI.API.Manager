using AI.API.Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// 分页结果实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class PagedList<T> : IPagedList<T>
{
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public IReadOnlyList<T> Items { get; }

    public PagedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items.ToList().AsReadOnly();
    }

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, count, pageIndex, pageSize);
    }
}