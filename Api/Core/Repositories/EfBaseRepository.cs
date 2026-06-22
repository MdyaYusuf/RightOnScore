using System.Linq.Expressions;
using Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Repositories;

public class EfBaseRepository<TContext, TEntity, TId> : IRepository<TEntity, TId>
  where TEntity : Entity<TId>, new()
  where TContext : DbContext
  where TId : notnull
{
  protected TContext _context { get; }

  public EfBaseRepository(TContext context)
  {
    _context = context;
  }

  public async Task<List<TEntity>> GetAllAsync(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    IQueryable<TEntity> query = _context.Set<TEntity>();

    if (!enableTracking)
    {
      query = query.AsNoTracking();
    }

    if (withDeleted)
    {
      query = query.IgnoreQueryFilters();
    }

    if (include != null)
    {
      query = include(query);
    }

    if (filter != null)
    {
      query = query.Where(filter);
    }

    if (orderBy != null)
    {
      query = orderBy(query);
    }

    return await query.ToListAsync(cancellationToken);
  }

  public async Task<(List<TEntity> Items, int TotalCount)> GetPagedListAsync(
    int pageNumber,
    int pageSize,
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    IQueryable<TEntity> query = Query(enableTracking, withDeleted);

    if (filter != null)
    {
      query = query.Where(filter);
    }

    if (include != null)
    {
      query = include(query);
    }

    var totalCount = await query.CountAsync(cancellationToken);

    if (orderBy != null)
    {
      query = orderBy(query);
    }

    var items = await query
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync(cancellationToken);

    return (items, totalCount);
  }

  public async Task<TEntity?> GetAsync(
    Expression<Func<TEntity, bool>> predicate,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    IQueryable<TEntity> query = _context.Set<TEntity>();

    if (!enableTracking)
    {
      query = query.AsNoTracking();
    }

    if (include != null)
    {
      query = include(query);
    }

    return await query.FirstOrDefaultAsync(predicate, cancellationToken);
  }

  public async Task<TEntity?> GetByIdAsync(
    TId id,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    IQueryable<TEntity> query = _context.Set<TEntity>();

    if (!enableTracking)
    {
      query = query.AsNoTracking();
    }

    if (include != null)
    {
      query = include(query);
    }

    return await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
  }

  public async Task<bool> AnyAsync(
    Expression<Func<TEntity, bool>> predicate,
    CancellationToken cancellationToken = default)
  {
    return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
  }

  public async Task<TEntity> AddAsync(
    TEntity entity,
    CancellationToken cancellationToken)
  {
    entity.CreatedDate = DateTime.Now;
    await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

    return entity;
  }

  public IQueryable<TEntity> Query(bool enableTracking = false, bool withDeleted = false)
  {
    IQueryable<TEntity> query = _context.Set<TEntity>();

    if (!enableTracking)
    {
      query = query.AsNoTracking();
    }

    if (withDeleted)
    {
      query = query.IgnoreQueryFilters();
    }

    return query;
  }

  public void Delete(TEntity entity)
  {
    _context.Set<TEntity>().Remove(entity);
  }

  public void Update(TEntity entity)
  {
    entity.UpdatedDate = DateTime.Now;
    _context.Set<TEntity>().Update(entity);
  }
}