using Api.Core.Repositories;

namespace Api.Features.Competitions;

public interface ICompetitionRepository : IRepository<Competition, Guid>
{
  Task<List<Competition>> GetActiveCompetitionsAsync(CancellationToken cancellationToken = default);
  Task<Competition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
