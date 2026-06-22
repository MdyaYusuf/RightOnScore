namespace Api.Core.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}