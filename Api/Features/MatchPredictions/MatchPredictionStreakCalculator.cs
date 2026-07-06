namespace Api.Features.MatchPredictions;

public static class MatchPredictionStreakCalculator
{
  public static int CountPriorConsecutiveExactScores(
    IReadOnlyList<MatchPrediction> priorPredictions,
    DateTime currentKickoffTime)
  {
    if (priorPredictions.Count == 0)
    {
      return 0;
    }

    var ordered = priorPredictions
      .OrderBy(p => p.Match.KickoffTime)
      .ThenBy(p => p.Match.Id)
      .ToList();

    int consecutive = 0;

    for (int i = ordered.Count - 1; i >= 0; i--)
    {
      MatchPrediction prediction = ordered[i];
      bool exact = MatchPredictionScoringRules.IsExactScore(prediction, prediction.Match);

      if (exact)
      {
        consecutive++;
        continue;
      }

      if (prediction.Match.KickoffTime < currentKickoffTime)
      {
        break;
      }
    }

    return consecutive;
  }
}
