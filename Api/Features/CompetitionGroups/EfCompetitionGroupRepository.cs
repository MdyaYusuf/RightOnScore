using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionGroups;

public class EfCompetitionGroupRepository : EfBaseRepository<BaseDbContext, CompetitionGroup, Guid>, ICompetitionGroupRepository
{
  public EfCompetitionGroupRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<CompetitionGroup>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cg => cg.CompetitionStage)
      .Where(cg => cg.CompetitionStageId == competitionStageId)
      .OrderBy(cg => cg.DisplayOrder)
      .ThenBy(cg => cg.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionGroup>> GetActiveByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Include(cg => cg.CompetitionStage)
      .Where(cg => cg.CompetitionStageId == competitionStageId && cg.IsActive)
      .OrderBy(cg => cg.DisplayOrder)
      .ThenBy(cg => cg.Name)
      .ToListAsync(cancellationToken);
  }
}
