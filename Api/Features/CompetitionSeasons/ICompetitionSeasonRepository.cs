using Api.Core.Repositories;

namespace Api.Features.CompetitionSeasons;

public interface ICompetitionSeasonRepository : IRepository<CompetitionSeason, Guid>
{
  Task<List<CompetitionSeason>> GetByCompetitionIdAsync(Guid competitionId, CancellationToken cancellationToken = default);
  Task<List<CompetitionSeason>> GetActiveSeasonsAsync(CancellationToken cancellationToken = default);
  Task<CompetitionSeason?> GetActiveSeasonByCompetitionIdAsync(Guid competitionId, CancellationToken cancellationToken = default);
}
