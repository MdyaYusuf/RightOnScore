using Api.Core.Repositories;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.SeasonStandings;

public class EfSeasonStandingRepository
  : EfBaseRepository<BaseDbContext, SeasonStanding, Guid>, ISeasonStandingRepository
{
  public EfSeasonStandingRepository(BaseDbContext context) : base(context)
  {
  }

  public async Task<SeasonStanding?> GetByUserIdAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking)
      .IncludeDetails()
      .FirstOrDefaultAsync(
        s => s.UserId == userId && s.CompetitionSeasonId == competitionSeasonId,
        cancellationToken);
  }

  public async Task<List<SeasonStanding>> GetTopByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    int take,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(s => s.CompetitionSeasonId == competitionSeasonId)
      .OrderByDescending(s => s.TotalPoints)
      .ThenByDescending(s => s.ExactScoreCount)
      .ThenBy(s => s.User.Username)
      .Take(take)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<SeasonStanding>> GetOrderedByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(s => s.CompetitionSeasonId == competitionSeasonId)
      .OrderByDescending(s => s.TotalPoints)
      .ThenByDescending(s => s.ExactScoreCount)
      .ThenBy(s => s.User.Username)
      .ToListAsync(cancellationToken);
  }

  public async Task<int> GetParticipantCountByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .CountAsync(s => s.CompetitionSeasonId == competitionSeasonId, cancellationToken);
  }
}

public static class SeasonStandingQueryableExtensions
{
  public static IQueryable<SeasonStanding> IncludeDetails(this IQueryable<SeasonStanding> query)
  {
    return query
      .Include(s => s.User)
        .ThenInclude(u => u.Role)
      .Include(s => s.CompetitionSeason);
  }
}
