using AI.API.Manager.Domain.Repositories;
using System.Linq.Expressions;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// 查询规范实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class Specification<T> : ISpecification<T> where T : class
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<Expression<Func<T, object>>> OrderBy { get; } = new();
    public List<Expression<Func<T, object>>> OrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public Specification() { }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy.Add(orderByExpression);
    }

    public void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending.Add(orderByDescendingExpression);
    }

    public void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}