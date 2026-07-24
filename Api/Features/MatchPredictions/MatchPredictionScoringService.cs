using Api.Core.Exceptions;
using Api.Core.Repositories;
using Api.Features.SeasonStandings;
using Api.Features.CompetitionStages;
using Api.Features.Matches;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MatchPredictions;

public class MatchPredictionScoringService(
  IMatchRepository _matchRepository,
  IMatchPredictionRepository _matchPredictionRepository,
  ISeasonStandingService _seasonStandingService,
  MatchPredictionScoringBusinessRules _scoringBusinessRules,
  IUnitOfWork _unitOfWork) : IMatchPredictionScoringService
{
  public async Task ScoreMatchAsync(Guid matchId, CancellationToken cancellationToken = default)
  {
    Match match = await LoadMatchForScoringAsync(matchId, cancellationToken);

    IReadOnlyList<Guid> userIds = await ScoreMatchPredictionsAsync(match, cancellationToken);

    if (userIds.Count == 0)
    {
      return;
    }

    await _seasonStandingService.RefreshStandingsForUsersAsync(
      userIds,
      match.CompetitionSeasonId,
      cancellationToken);

    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }

  public async Task RescoreMatchAndLaterAsync(Guid matchId, CancellationToken cancellationToken = default)
  {
    Match match = await LoadMatchForScoringAsync(matchId, cancellationToken);

    List<Guid> matchIds = await _matchRepository.GetFinishedMatchIdsFromAsync(
      match.CompetitionSeasonId,
      match.KickoffTime,
      match.Id,
      cancellationToken);

    HashSet<Guid> affectedUserIds = [];

    foreach (Guid currentMatchId in matchIds)
    {
      Match currentMatch = currentMatchId == match.Id
        ? match
        : await LoadMatchForScoringAsync(currentMatchId, cancellationToken);

      IReadOnlyList<Guid> userIds = await ScoreMatchPredictionsAsync(currentMatch, cancellationToken);

      foreach (Guid userId in userIds)
      {
        affectedUserIds.Add(userId);
      }

      // Persist each match before the next so prior-streak queries see updated PointsEarned.
      if (userIds.Count > 0)
      {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
      }
    }

    if (affectedUserIds.Count == 0)
    {
      return;
    }

    await _seasonStandingService.RefreshStandingsForUsersAsync(
      affectedUserIds,
      match.CompetitionSeasonId,
      cancellationToken);

    await _unitOfWork.SaveChangesAsync(cancellationToken);
  }

  private async Task<Match> LoadMatchForScoringAsync(Guid matchId, CancellationToken cancellationToken)
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

    return match;
  }

  private async Task<IReadOnlyList<Guid>> ScoreMatchPredictionsAsync(
    Match match,
    CancellationToken cancellationToken)
  {
    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByMatchIdForScoringAsync(
      match.Id,
      cancellationToken);

    if (predictions.Count == 0)
    {
      return [];
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

    return predictions.Select(p => p.UserId).ToList();
  }
}
