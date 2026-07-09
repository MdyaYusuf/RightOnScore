using Api.Core.Repositories;

namespace Api.Features.MatchPredictions;

public interface IMatchPredictionRepository : IRepository<MatchPrediction, Guid>
{
  Task<MatchPrediction?> GetByUserIdAndMatchIdAsync(
    Guid userId,
    Guid matchId,
    CancellationToken cancellationToken = default);

  Task<List<MatchPrediction>> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default);

  Task<List<MatchPrediction>> GetByMatchIdAsync(
    Guid matchId,
    CancellationToken cancellationToken = default);

  Task<List<MatchPrediction>> GetByCompetitionSeasonIdAndUserIdAsync(
    Guid competitionSeasonId,
    Guid userId,
    CancellationToken cancellationToken = default);

  Task<int> GetTotalPointsByUserAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<int> GetScoredPredictionCountByUserAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<List<MatchPrediction>> GetByMatchIdForScoringAsync(
    Guid matchId,
    CancellationToken cancellationToken = default);

  Task<List<MatchPrediction>> GetPriorScoredPredictionsForStreakAsync(
    Guid userId,
    Guid competitionSeasonId,
    DateTime kickoffTime,
    Guid matchId,
    CancellationToken cancellationToken = default);

  Task<PredictionStandingStats> GetStandingStatsForUserAndSeasonAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);
}
