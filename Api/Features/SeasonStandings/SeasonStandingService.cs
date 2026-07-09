using Api.Core.Responses;
using Api.Features.MatchPredictions;

namespace Api.Features.SeasonStandings;

public class SeasonStandingService(
  ISeasonStandingRepository _seasonStandingRepository,
  IMatchPredictionRepository _matchPredictionRepository,
  SeasonStandingMapper _mapper,
  SeasonStandingBusinessRules _businessRules) : ISeasonStandingService
{
  public async Task<ReturnModel<List<SeasonStandingPreviewDto>>> GetTopByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    int topCount = 50,
    CancellationToken cancellationToken = default)
  {
    _businessRules.TopCountMustBeValid(topCount);
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<SeasonStanding> standings = await _seasonStandingRepository.GetTopByCompetitionSeasonIdAsync(
      competitionSeasonId,
      topCount,
      cancellationToken);

    List<SeasonStandingPreviewDto> responseDtos = MapWithRanks(standings, startRank: 1);

    return new ReturnModel<List<SeasonStandingPreviewDto>>()
    {
      Success = true,
      Message = "Sezon sıralaması başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MySeasonStandingDto>> GetMineByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    SeasonStanding? standing = await _seasonStandingRepository.GetByUserIdAndCompetitionSeasonIdAsync(
      currentUserId,
      competitionSeasonId,
      cancellationToken: cancellationToken);

    if (standing == null)
    {
      return new ReturnModel<MySeasonStandingDto>()
      {
        Success = true,
        Message = "Bu sezon için sıralama kaydınız bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    int rank = await CalculateRankAsync(standing, competitionSeasonId, cancellationToken);
    int totalParticipants = await _seasonStandingRepository.GetParticipantCountByCompetitionSeasonIdAsync(
      competitionSeasonId,
      cancellationToken);

    var response = new MySeasonStandingDto
    {
      CompetitionSeasonId = competitionSeasonId,
      Rank = rank,
      TotalPoints = standing.TotalPoints,
      ExactScoreCount = standing.ExactScoreCount,
      ScoredPredictionCount = standing.ScoredPredictionCount,
      TotalParticipants = totalParticipants
    };

    return new ReturnModel<MySeasonStandingDto>()
    {
      Success = true,
      Message = "Sezon sıralamanız başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task RefreshStandingsForUsersAsync(
    IEnumerable<Guid> userIds,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    foreach (Guid userId in userIds.Distinct())
    {
      PredictionStandingStats stats = await _matchPredictionRepository.GetStandingStatsForUserAndSeasonAsync(
        userId,
        competitionSeasonId,
        cancellationToken);

      if (stats.ScoredPredictionCount == 0)
      {
        continue;
      }

      SeasonStanding? standing = await _seasonStandingRepository.GetByUserIdAndCompetitionSeasonIdAsync(
        userId,
        competitionSeasonId,
        enableTracking: true,
        cancellationToken: cancellationToken);

      if (standing == null)
      {
        standing = new SeasonStanding
        {
          UserId = userId,
          CompetitionSeasonId = competitionSeasonId
        };

        await _seasonStandingRepository.AddAsync(standing, cancellationToken);
      }

      standing.TotalPoints = stats.TotalPoints;
      standing.ExactScoreCount = stats.ExactScoreCount;
      standing.ScoredPredictionCount = stats.ScoredPredictionCount;

      _seasonStandingRepository.Update(standing);
    }
  }

  private async Task<int> CalculateRankAsync(
    SeasonStanding standing,
    Guid competitionSeasonId,
    CancellationToken cancellationToken)
  {
    List<SeasonStanding> orderedStandings = await _seasonStandingRepository.GetOrderedByCompetitionSeasonIdAsync(
      competitionSeasonId,
      cancellationToken);

    int rank = 1;

    foreach (SeasonStanding entry in orderedStandings)
    {
      if (entry.Id == standing.Id)
      {
        return rank;
      }

      rank++;
    }

    return rank;
  }

  private List<SeasonStandingPreviewDto> MapWithRanks(
    List<SeasonStanding> standings,
    int startRank)
  {
    var responseDtos = new List<SeasonStandingPreviewDto>(standings.Count);
    int rank = startRank;

    foreach (SeasonStanding standing in standings)
    {
      SeasonStandingPreviewDto dto = _mapper.EntityToPreviewDto(standing) with { Rank = rank };
      responseDtos.Add(dto);
      rank++;
    }

    return responseDtos;
  }
}
