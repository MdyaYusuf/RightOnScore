using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.CompetitionStages;

public interface ICompetitionStageService
{
  Task<ReturnModel<PagedResponse<CompetitionStageResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionStage, bool>>? filter = null,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    Func<IQueryable<CompetitionStage>, IOrderedQueryable<CompetitionStage>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionStagePreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionStagePreviewDto>>> GetActiveByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionStageResponseDto>> GetAsync(
    Expression<Func<CompetitionStage, bool>> predicate,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionStageResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCompetitionStageResponseDto>> AddAsync(
    CreateCompetitionStageRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionStageRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionStageStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
