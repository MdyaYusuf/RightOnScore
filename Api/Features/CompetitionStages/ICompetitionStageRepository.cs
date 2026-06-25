using Api.Core.Repositories;

namespace Api.Features.CompetitionStages;

public interface ICompetitionStageRepository : IRepository<CompetitionStage, Guid>
{
  Task<List<CompetitionStage>> GetByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
  Task<List<CompetitionStage>> GetActiveByCompetitionSeasonIdAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default);
}
