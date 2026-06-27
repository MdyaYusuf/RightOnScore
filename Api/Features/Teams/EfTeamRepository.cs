using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Teams;

public class EfTeamRepository : EfBaseRepository<BaseDbContext, Team, Guid>, ITeamRepository
{
  public EfTeamRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Where(t => t.IsActive)
      .OrderBy(t => t.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Team>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Where(t => t.Country == country)
      .OrderBy(t => t.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Team>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Where(t => t.Name.Contains(searchTerm) || t.ShortName.Contains(searchTerm))
      .OrderBy(t => t.Name)
      .ToListAsync(cancellationToken);
  }
}
