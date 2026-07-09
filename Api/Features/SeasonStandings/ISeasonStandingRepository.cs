using Api.Core.Repositories;

namespace Api.Features.SeasonStandings;

public interface ISeasonStandingRepository : IRepository<SeasonStanding, Guid>
{
  Task<SeasonStanding?> GetByUserIdAndCompetitionSeasonIdAsync(
    Guid userId,
    Guid competitionSeasonId,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<List<SeasonStanding>> GetTopByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    int take,
    CancellationToken cancellationToken = default);

  Task<List<SeasonStanding>> GetOrderedByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<int> GetParticipantCountByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);
}
