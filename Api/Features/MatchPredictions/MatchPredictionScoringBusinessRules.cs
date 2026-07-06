using Api.Core.Exceptions;
using Api.Features.CompetitionStages;
using Api.Features.Matches;

namespace Api.Features.MatchPredictions;

public class MatchPredictionScoringBusinessRules
{
  public void MatchMustBeFinishedWithScores(Match match)
  {
    if (match.Status != MatchStatus.Finished)
    {
      throw new BusinessException("Yalnızca tamamlanmış maçlar için skor tahminleri puanlanabilir.");
    }

    if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
    {
      throw new BusinessException("Puanlama için maç skorları kayıtlı olmalıdır.");
    }
  }

  public void AdvancingTeamMustBeValidForKnockoutDraw(
    CompetitionStage? stage,
    int homeScore,
    int awayScore,
    Guid? advancingTeamId,
    Guid homeTeamId,
    Guid awayTeamId)
  {
    if (!MatchPredictionScoringRules.IsKnockoutStage(stage))
    {
      return;
    }

    if (homeScore != awayScore)
    {
      if (advancingTeamId.HasValue)
      {
        throw new BusinessException("Beraberlik olmayan maçlarda tur atlayan takım belirtilmemelidir.");
      }

      return;
    }

    if (!advancingTeamId.HasValue)
    {
      throw new BusinessException("Eleme maçlarında beraberlik sonucunda tur atlayan takım belirtilmelidir.");
    }

    if (advancingTeamId.Value != homeTeamId && advancingTeamId.Value != awayTeamId)
    {
      throw new BusinessException("Tur atlayan takım ev sahibi veya deplasman takımlarından biri olmalıdır.");
    }
  }
}
