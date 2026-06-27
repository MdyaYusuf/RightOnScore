using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.CompetitionGroups;

public interface ICompetitionGroupService
{
  Task<ReturnModel<PagedResponse<CompetitionGroupResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionGroup, bool>>? filter = null,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    Func<IQueryable<CompetitionGroup>, IOrderedQueryable<CompetitionGroup>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionGroupPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionGroupPreviewDto>>> GetActiveByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionGroupResponseDto>> GetAsync(
    Expression<Func<CompetitionGroup, bool>> predicate,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionGroupResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCompetitionGroupResponseDto>> AddAsync(
    CreateCompetitionGroupRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionGroupRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionGroupStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
