using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionTeams;

public class EfCompetitionTeamRepository : EfBaseRepository<BaseDbContext, CompetitionTeam, Guid>, ICompetitionTeamRepository
{
  public EfCompetitionTeamRepository(BaseDbContext context) : base(context)
  {

  }

  public async Task<List<CompetitionTeam>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(ct => ct.CompetitionSeasonId == competitionSeasonId)
      .OrderBy(ct => ct.Seed ?? int.MaxValue)
      .ThenBy(ct => ct.Team.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionTeam>> GetActiveByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(ct => ct.CompetitionSeasonId == competitionSeasonId && ct.IsActive)
      .OrderBy(ct => ct.Seed ?? int.MaxValue)
      .ThenBy(ct => ct.Team.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionTeam>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(ct => ct.CompetitionStageId == competitionStageId)
      .OrderBy(ct => ct.Seed ?? int.MaxValue)
      .ThenBy(ct => ct.Team.Name)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<CompetitionTeam>> GetByCompetitionGroupIdAsync(Guid competitionGroupId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(ct => ct.CompetitionGroupId == competitionGroupId)
      .OrderBy(ct => ct.Seed ?? int.MaxValue)
      .ThenBy(ct => ct.Team.Name)
      .ToListAsync(cancellationToken);
  }
}

public static class CompetitionTeamQueryableExtensions
{
  public static IQueryable<CompetitionTeam> IncludeDetails(this IQueryable<CompetitionTeam> query)
  {
    return query
      .Include(ct => ct.CompetitionSeason)
      .Include(ct => ct.Team)
      .Include(ct => ct.CompetitionStage)
      .Include(ct => ct.CompetitionGroup);
  }
}
