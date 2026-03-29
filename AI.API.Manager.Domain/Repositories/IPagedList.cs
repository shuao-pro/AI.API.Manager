namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// 分页结果接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IPagedList<T>
{
    /// <summary>
    /// 当前页码
    /// </summary>
    int PageIndex { get; }

    /// <summary>
    /// 每页大小
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// 总记录数
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// 总页数
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// 是否有下一页
    /// </summary>
    bool HasNextPage { get; }

    /// <summary>
    /// 数据列表
    /// </summary>
    IReadOnlyList<T> Items { get; }
}