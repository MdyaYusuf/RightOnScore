using Api.Core.Repositories;

namespace Api.Features.Matches;

public interface IMatchRepository : IRepository<Match, Guid>
{
  Task<List<Match>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
  Task<List<Match>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default);
  Task<List<Match>> GetByCompetitionGroupIdAsync(Guid competitionGroupId, CancellationToken cancellationToken = default);
  Task<List<Match>> GetUpcomingByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
}
