using Api.Core.Repositories;

namespace Api.Data;

public class UnitOfWork(BaseDbContext _context) : IUnitOfWork
{
  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }

  public async ValueTask DisposeAsync()
  {
    await _context.DisposeAsync();
  }
}