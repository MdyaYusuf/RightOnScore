using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.Matches;

public interface IMatchService
{
  Task<ReturnModel<PagedResponse<MatchResponseDto>>> GetAllAsync(
    Expression<Func<Match, bool>>? filter = null,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    Func<IQueryable<Match>, IOrderedQueryable<Match>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPreviewDto>>> GetUpcomingByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionGroupIdAsync(
    Guid competitionGroupId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<MatchResponseDto>> GetAsync(
    Expression<Func<Match, bool>> predicate,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<MatchResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedMatchResponseDto>> AddAsync(
    CreateMatchRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateMatchRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeMatchStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RecordResultAsync(
    RecordMatchResultRequest request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);
}
