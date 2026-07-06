using Api.Core.Exceptions;
using Api.Core.Repositories;
using Api.Features.CompetitionStages;
using Api.Features.Matches;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MatchPredictions;

public class MatchPredictionScoringService(
  IMatchRepository _matchRepository,
  IMatchPredictionRepository _matchPredictionRepository,
  MatchPredictionScoringBusinessRules _scoringBusinessRules,
  IUnitOfWork _unitOfWork) : IMatchPredictionScoringService
{
  public async Task ScoreMatchAsync(Guid matchId, CancellationToken cancellationToken = default)
  {
    Match match = await _matchRepository.GetByIdAsync(
      matchId,
      include: query => query
        .Include(m => m.CompetitionStage)
        .Include(m => m.HomeTeam)
        .Include(m => m.AwayTeam)
        .Include(m => m.AdvancingTeam),
      enableTracking: true,
      cancellationToken: cancellationToken)
      ?? throw new NotFoundException($"{matchId} numaralı maç bulunamadı.");

    _scoringBusinessRules.MatchMustBeFinishedWithScores(match);
    _scoringBusinessRules.AdvancingTeamMustBeValidForKnockoutDraw(
      match.CompetitionStage,
      match.HomeScore!.Value,
      match.AwayScore!.Value,
      match.AdvancingTeamId,
      match.HomeTeamId,
      match.AwayTeamId);

    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByMatchIdForScoringAsync(
      matchId,
      cancellationToken);

    if (predictions.Count == 0)
    {
      return;
    }

    CompetitionStage? stage = match.CompetitionStage;

    foreach (MatchPrediction prediction in predictions)
    {
      List<MatchPrediction> priorPredictions = await _matchPredictionRepository
        .GetPriorScoredPredictionsForStreakAsync(
          prediction.UserId,
          match.CompetitionSeasonId,
          match.KickoffTime,
          match.Id,
          cancellationToken);

      int priorExactStreak = MatchPredictionStreakCalculator.CountPriorConsecutiveExactScores(
        priorPredictions,
        match.KickoffTime);

      prediction.PointsEarned = MatchPredictionScoringRules.CalculatePoints(
        prediction,
        match,
        stage,
        priorExactStreak);

      _matchPredictionRepository.Update(prediction);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }
}
