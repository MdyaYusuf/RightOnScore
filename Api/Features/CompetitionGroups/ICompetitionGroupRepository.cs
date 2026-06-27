using Api.Core.Repositories;

namespace Api.Features.CompetitionGroups;

public interface ICompetitionGroupRepository : IRepository<CompetitionGroup, Guid>
{
  Task<List<CompetitionGroup>> GetByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default);
  Task<List<CompetitionGroup>> GetActiveByCompetitionStageIdAsync(Guid competitionStageId, CancellationToken cancellationToken = default);
}
