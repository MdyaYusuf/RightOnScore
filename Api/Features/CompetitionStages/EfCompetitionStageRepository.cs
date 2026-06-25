using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionStages;

public class EfCompetitionStageRepository : EfBaseRepository<BaseDbContext, CompetitionStage, Guid>, ICompetitionStageRepository
{
  public EfCompetitionStageRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<CompetitionStage>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cs => cs.CompetitionSeason)
      .Where(cs => cs.CompetitionSeasonId == competitionSeasonId)
      .OrderBy(cs => cs.DisplayOrder)
      .ThenBy(cs => cs.StartDate)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionStage>> GetActiveByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cs => cs.CompetitionSeason)
      .Where(cs => cs.CompetitionSeasonId == competitionSeasonId && cs.IsActive)
      .OrderBy(cs => cs.DisplayOrder)
      .ThenBy(cs => cs.StartDate)
      .ToListAsync(cancellationToken);
  }
}
