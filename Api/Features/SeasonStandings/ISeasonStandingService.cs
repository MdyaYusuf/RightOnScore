using Api.Core.Responses;

namespace Api.Features.SeasonStandings;

public interface ISeasonStandingService
{
  Task<ReturnModel<List<SeasonStandingPreviewDto>>> GetTopByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    int topCount = 50,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<MySeasonStandingDto>> GetMineByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default);

  Task RefreshStandingsForUsersAsync(
    IEnumerable<Guid> userIds,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);
}
