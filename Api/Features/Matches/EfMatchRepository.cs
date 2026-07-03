using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Matches;

public class EfMatchRepository : EfBaseRepository<BaseDbContext, Match, Guid>, IMatchRepository
{
  public EfMatchRepository(BaseDbContext context) : base(context)
  {
  }

  public async Task<List<Match>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(m => m.CompetitionSeasonId == competitionSeasonId)
      .OrderBy(m => m.KickoffTime)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Match>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(m => m.CompetitionStageId == competitionStageId)
      .OrderBy(m => m.KickoffTime)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Match>> GetByCompetitionGroupIdAsync(Guid competitionGroupId, CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(m => m.CompetitionGroupId == competitionGroupId)
      .OrderBy(m => m.KickoffTime)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<Match>> GetUpcomingByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var now = DateTime.Now;

    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(m => m.CompetitionSeasonId == competitionSeasonId
        && m.Status == MatchStatus.Scheduled
        && m.KickoffTime > now)
      .OrderBy(m => m.KickoffTime)
      .ToListAsync(cancellationToken);
  }
}

public static class MatchQueryableExtensions
{
  public static IQueryable<Match> IncludeDetails(this IQueryable<Match> query)
  {
    return query
      .Include(m => m.CompetitionSeason)
      .Include(m => m.CompetitionStage)
      .Include(m => m.CompetitionGroup)
      .Include(m => m.HomeTeam)
      .Include(m => m.AwayTeam);
  }
}
