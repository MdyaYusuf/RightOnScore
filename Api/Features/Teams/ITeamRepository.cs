using Api.Core.Repositories;

namespace Api.Features.Teams;

public interface ITeamRepository : IRepository<Team, Guid>
{
  Task<List<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default);
  Task<List<Team>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
  Task<List<Team>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
