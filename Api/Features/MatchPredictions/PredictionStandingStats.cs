namespace Api.Features.MatchPredictions;

public sealed record PredictionStandingStats(
  int TotalPoints,
  int ExactScoreCount,
  int ScoredPredictionCount);
