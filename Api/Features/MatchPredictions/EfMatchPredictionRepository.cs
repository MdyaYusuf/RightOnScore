using Api.Core.Repositories;
using Api.Data;
using Api.Features.Matches;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MatchPredictions;

public class EfMatchPredictionRepository
  : EfBaseRepository<BaseDbContext, MatchPrediction, Guid>, IMatchPredictionRepository
{
  public EfMatchPredictionRepository(BaseDbContext context) : base(context)
  {
  }

  public async Task<MatchPrediction?> GetByUserIdAndMatchIdAsync(
    Guid userId,
    Guid matchId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .FirstOrDefaultAsync(p => p.UserId == userId && p.MatchId == matchId, cancellationToken);
  }

  public async Task<List<MatchPrediction>> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(p => p.UserId == userId)
      .OrderByDescending(p => p.Match.KickoffTime)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<MatchPrediction>> GetByMatchIdAsync(
    Guid matchId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(p => p.MatchId == matchId)
      .OrderBy(p => p.User.Username)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<MatchPrediction>> GetByCompetitionSeasonIdAndUserIdAsync(
    Guid competitionSeasonId,
    Guid userId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeDetails()
      .Where(p => p.UserId == userId && p.Match.CompetitionSeasonId == competitionSeasonId)
      .OrderBy(p => p.Match.KickoffTime)
      .ToListAsync(cancellationToken);
  }

  public async Task<int> GetTotalPointsByUserAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .Where(p => p.UserId == userId
        && p.Match.CompetitionSeasonId == competitionSeasonId
        && p.PointsEarned != null)
      .SumAsync(p => p.PointsEarned!.Value, cancellationToken);
  }

  public async Task<int> GetScoredPredictionCountByUserAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .CountAsync(p => p.UserId == userId
        && p.Match.CompetitionSeasonId == competitionSeasonId
        && p.PointsEarned != null, cancellationToken);
  }

  public async Task<List<MatchPrediction>> GetByMatchIdForScoringAsync(
    Guid matchId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: true)
      .IncludeForScoring()
      .Where(p => p.MatchId == matchId)
      .ToListAsync(cancellationToken);
  }

  public async Task<List<MatchPrediction>> GetPriorScoredPredictionsForStreakAsync(
    Guid userId,
    Guid competitionSeasonId,
    DateTime kickoffTime,
    Guid matchId,
    CancellationToken cancellationToken = default)
  {
    return await Query(enableTracking: false)
      .IncludeForScoring()
      .Where(p => p.UserId == userId
        && p.Match.CompetitionSeasonId == competitionSeasonId
        && p.PointsEarned != null
        && p.Match.Status == MatchStatus.Finished
        && (p.Match.KickoffTime < kickoffTime
          || (p.Match.KickoffTime == kickoffTime && p.Match.Id < matchId)))
      .ToListAsync(cancellationToken);
  }

  public async Task<PredictionStandingStats> GetStandingStatsForUserAndSeasonAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    List<MatchPrediction> predictions = await Query(enableTracking: false)
      .IncludeForScoring()
      .Where(p => p.UserId == userId
        && p.Match.CompetitionSeasonId == competitionSeasonId
        && p.PointsEarned != null
        && p.Match.Status == MatchStatus.Finished)
      .ToListAsync(cancellationToken);

    if (predictions.Count == 0)
    {
      return new PredictionStandingStats(0, 0, 0);
    }

    int totalPoints = predictions.Sum(p => p.PointsEarned!.Value);
    int exactScoreCount = predictions.Count(p => MatchPredictionScoringRules.IsExactScore(p, p.Match));

    return new PredictionStandingStats(totalPoints, exactScoreCount, predictions.Count);
  }
}

public static class MatchPredictionQueryableExtensions
{
  public static IQueryable<MatchPrediction> IncludeDetails(this IQueryable<MatchPrediction> query)
  {
    return query
      .Include(p => p.User)
        .ThenInclude(u => u.Role)
      .Include(p => p.Match)
        .ThenInclude(m => m.HomeTeam)
      .Include(p => p.Match)
        .ThenInclude(m => m.AwayTeam);
  }

  public static IQueryable<MatchPrediction> IncludeForScoring(this IQueryable<MatchPrediction> query)
  {
    return query
      .Include(p => p.Match)
        .ThenInclude(m => m.CompetitionStage)
      .Include(p => p.Match)
        .ThenInclude(m => m.HomeTeam)
      .Include(p => p.Match)
        .ThenInclude(m => m.AwayTeam);
  }
}
