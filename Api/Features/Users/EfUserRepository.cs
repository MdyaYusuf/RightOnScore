using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

public class EfUserRepository : EfBaseRepository<BaseDbContext, User, Guid>, IUserRepository
{
  public EfUserRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<bool> IsEmailUniqueAsync(
    string email,
    CancellationToken cancellationToken = default)
  {
    return !await Query(enableTracking: false, withDeleted: true)
      .AnyAsync(u => u.Email == email, cancellationToken);
  }

  public async Task<List<User>> GetNewestMembersAsync(
    int count,
    DateTime? lastDateCursor = null,
    Guid? lastIdCursor = null,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    IQueryable<User> query = Query(enableTracking, withDeleted);

    if (include != null)
    {
      query = include(query);
    }

    if (lastDateCursor.HasValue && lastIdCursor.HasValue)
    {
      query = query.Where(u => u.CreatedDate < lastDateCursor ||
                              (u.CreatedDate == lastDateCursor && u.Id.CompareTo(lastIdCursor.Value) < 0));
    }

    return await query
      .Where(u => u.IsActive)
      .OrderByDescending(u => u.CreatedDate).ThenByDescending(u => u.Id)
      .Take(count)
      .ToListAsync(cancellationToken);
  }
}