using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionSeasons;

public class EfCompetitionSeasonRepository : EfBaseRepository<BaseDbContext, CompetitionSeason, Guid>, ICompetitionSeasonRepository
{
  public EfCompetitionSeasonRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<CompetitionSeason>> GetByCompetitionIdAsync(Guid competitionId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cs => cs.Competition)
      .Where(cs => cs.CompetitionId == competitionId)
      .OrderByDescending(cs => cs.StartDate)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionSeason>> GetActiveSeasonsAsync(CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cs => cs.Competition)
      .Where(cs => cs.IsActive)
      .OrderBy(cs => cs.Competition.Name)
      .ThenByDescending(cs => cs.StartDate)
      .ToListAsync(cancellationToken);
  }

  public async Task<CompetitionSeason?> GetActiveSeasonByCompetitionIdAsync(Guid competitionId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cs => cs.Competition)
      .FirstOrDefaultAsync(cs => cs.CompetitionId == competitionId && cs.IsActive, cancellationToken);
  }
}
