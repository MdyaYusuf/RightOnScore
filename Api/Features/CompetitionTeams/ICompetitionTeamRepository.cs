using Api.Core.Repositories;

namespace Api.Features.CompetitionTeams;

public interface ICompetitionTeamRepository : IRepository<CompetitionTeam, Guid>
{
  Task<List<CompetitionTeam>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
  Task<List<CompetitionTeam>> GetActiveByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
  Task<List<CompetitionTeam>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default);
  Task<List<CompetitionTeam>> GetByCompetitionGroupIdAsync(Guid competitionGroupId, CancellationToken cancellationToken = default);
}
