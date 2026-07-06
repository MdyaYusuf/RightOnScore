using Api.Features.CompetitionStages;
using Api.Features.Matches;

namespace Api.Features.MatchPredictions;

public static class MatchPredictionScoringRules
{
  public const int ExactScorePoints = 3;
  public const int CorrectResultPoints = 1;
  public const int CorrectAdvancerPoints = 1;
  public const int MaxStreakBonus = 2;

  public static bool IsKnockoutStage(CompetitionStage? stage) =>
    stage?.Type is CompetitionStageType.Knockout or CompetitionStageType.Final;

  public static bool IsExactScore(MatchPrediction prediction, Match match) =>
    match.HomeScore.HasValue
    && match.AwayScore.HasValue
    && prediction.PredictedHomeScore == match.HomeScore.Value
    && prediction.PredictedAwayScore == match.AwayScore.Value;

  public static bool IsCorrectResult(MatchPrediction prediction, Match match)
  {
    if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
    {
      return false;
    }

    return GetResultSign(prediction.PredictedHomeScore, prediction.PredictedAwayScore)
      == GetResultSign(match.HomeScore.Value, match.AwayScore.Value);
  }

  public static Guid? GetPredictedAdvancingTeamId(MatchPrediction prediction, Match match)
  {
    if (prediction.PredictedHomeScore > prediction.PredictedAwayScore)
    {
      return match.HomeTeamId;
    }

    if (prediction.PredictedAwayScore > prediction.PredictedHomeScore)
    {
      return match.AwayTeamId;
    }

    return prediction.PredictedAdvancingTeamId;
  }

  public static Guid? GetActualAdvancingTeamId(Match match)
  {
    if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
    {
      return null;
    }

    if (match.HomeScore.Value > match.AwayScore.Value)
    {
      return match.HomeTeamId;
    }

    if (match.AwayScore.Value > match.HomeScore.Value)
    {
      return match.AwayTeamId;
    }

    return match.AdvancingTeamId;
  }

  public static bool IsCorrectAdvancer(MatchPrediction prediction, Match match, CompetitionStage? stage)
  {
    if (!IsKnockoutStage(stage))
    {
      return false;
    }

    Guid? predictedAdvancer = GetPredictedAdvancingTeamId(prediction, match);
    Guid? actualAdvancer = GetActualAdvancingTeamId(match);

    return predictedAdvancer.HasValue
      && actualAdvancer.HasValue
      && predictedAdvancer.Value == actualAdvancer.Value;
  }

  public static int CalculatePoints(
    MatchPrediction prediction,
    Match match,
    CompetitionStage? stage,
    int priorExactStreak)
  {
    int points = 0;
    bool exact = IsExactScore(prediction, match);

    if (exact)
    {
      points += ExactScorePoints;
      points += Math.Min(priorExactStreak, MaxStreakBonus);
    }
    else if (IsCorrectResult(prediction, match))
    {
      points += CorrectResultPoints;
    }

    if (IsCorrectAdvancer(prediction, match, stage))
    {
      points += CorrectAdvancerPoints;
    }

    return points;
  }

  private static int GetResultSign(int homeScore, int awayScore) =>
    homeScore > awayScore ? 1 : homeScore < awayScore ? -1 : 0;
}
