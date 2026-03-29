using System.Linq.Expressions;

namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// 查询规范接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// 查询条件
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// 包含的关联实体
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// 排序表达式
    /// </summary>
    List<Expression<Func<T, object>>> OrderBy { get; }

    /// <summary>
    /// 降序排序表达式
    /// </summary>
    List<Expression<Func<T, object>>> OrderByDescending { get; }

    /// <summary>
    /// 分页大小
    /// </summary>
    int Take { get; }

    /// <summary>
    /// 跳过的记录数
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// 是否启用分页
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// 添加包含关联实体
    /// </summary>
    void AddInclude(Expression<Func<T, object>> includeExpression);

    /// <summary>
    /// 添加排序
    /// </summary>
    void AddOrderBy(Expression<Func<T, object>> orderByExpression);

    /// <summary>
    /// 添加降序排序
    /// </summary>
    void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);

    /// <summary>
    /// 应用分页
    /// </summary>
    void ApplyPaging(int skip, int take);
}