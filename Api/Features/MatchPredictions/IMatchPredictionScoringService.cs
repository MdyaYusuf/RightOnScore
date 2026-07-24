namespace Api.Features.MatchPredictions;

public interface IMatchPredictionScoringService
{
  Task ScoreMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
  Task RescoreMatchAndLaterAsync(Guid matchId, CancellationToken cancellationToken = default);
}
