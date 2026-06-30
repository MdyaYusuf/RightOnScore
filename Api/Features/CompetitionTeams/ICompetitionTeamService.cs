using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.CompetitionTeams;

public interface ICompetitionTeamService
{
  Task<ReturnModel<PagedResponse<CompetitionTeamResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionTeam, bool>>? filter = null,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    Func<IQueryable<CompetitionTeam>, IOrderedQueryable<CompetitionTeam>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetActiveByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionGroupIdAsync(
    Guid competitionGroupId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionTeamResponseDto>> GetAsync(
    Expression<Func<CompetitionTeam, bool>> predicate,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionTeamResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCompetitionTeamResponseDto>> AddAsync(
    CreateCompetitionTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionTeamStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
