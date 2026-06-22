using System.Linq.Expressions;
using Api.Core.Entities;

namespace Api.Core.Repositories;

public interface IRepository<TEntity, TId>
where TEntity : Entity<TId>, new()
where TId : notnull
{
  Task<List<TEntity>> GetAllAsync(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<(List<TEntity> Items, int TotalCount)> GetPagedListAsync(
    int pageNumber,
    int pageSize,
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<TEntity?> GetAsync(
    Expression<Func<TEntity, bool>> predicate,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<TEntity?> GetByIdAsync(
    TId id,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<bool> AnyAsync(
    Expression<Func<TEntity, bool>> predicate,
    CancellationToken cancellationToken = default);

  IQueryable<TEntity> Query(bool enableTracking = true, bool withDeleted = false);

  Task<TEntity> AddAsync(
    TEntity entity,
    CancellationToken cancellationToken = default);

  void Delete(TEntity entity);
  void Update(TEntity entity);
}