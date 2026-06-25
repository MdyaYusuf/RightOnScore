using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.CompetitionSeasons;

public interface ICompetitionSeasonService
{
  Task<ReturnModel<PagedResponse<CompetitionSeasonResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionSeason, bool>>? filter = null,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    Func<IQueryable<CompetitionSeason>, IOrderedQueryable<CompetitionSeason>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionSeasonPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionSeasonPreviewDto>>> GetByCompetitionIdAsync(
    Guid competitionId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionSeasonResponseDto>> GetActiveByCompetitionIdAsync(
    Guid competitionId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionSeasonResponseDto>> GetAsync(
    Expression<Func<CompetitionSeason, bool>> predicate,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionSeasonResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCompetitionSeasonResponseDto>> AddAsync(
    CreateCompetitionSeasonRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionSeasonRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionSeasonStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
