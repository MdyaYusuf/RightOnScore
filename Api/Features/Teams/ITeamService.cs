using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.Teams;

public interface ITeamService
{
  Task<ReturnModel<PagedResponse<TeamResponseDto>>> GetAllAsync(
    Expression<Func<Team, bool>>? filter = null,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    Func<IQueryable<Team>, IOrderedQueryable<Team>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<TeamPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default);

  Task<ReturnModel<List<TeamPreviewDto>>> GetByCountryAsync(
    string country,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<TeamPreviewDto>>> SearchAsync(
    SearchTeamRequest request,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<TeamResponseDto>> GetAsync(
    Expression<Func<Team, bool>> predicate,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<TeamResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedTeamResponseDto>> AddAsync(
    CreateTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeTeamStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
