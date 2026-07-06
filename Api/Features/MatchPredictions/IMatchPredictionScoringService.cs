namespace Api.Features.MatchPredictions;

public interface IMatchPredictionScoringService
{
  Task ScoreMatchAsync(Guid matchId, CancellationToken cancellationToken = default);
}
