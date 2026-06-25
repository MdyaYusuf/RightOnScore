using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.Competitions;

public interface ICompetitionService
{
  Task<ReturnModel<PagedResponse<CompetitionResponseDto>>> GetAllAsync(
    Expression<Func<Competition, bool>>? filter = null,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    Func<IQueryable<Competition>, IOrderedQueryable<Competition>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<CompetitionPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionResponseDto>> GetAsync(
    Expression<Func<Competition, bool>> predicate,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CompetitionResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCompetitionResponseDto>> AddAsync(
    CreateCompetitionRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
