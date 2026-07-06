using System.Linq.Expressions;
using Api.Core.Responses;

namespace Api.Features.MatchPredictions;

public interface IMatchPredictionService
{
  Task<ReturnModel<PagedResponse<MatchPredictionResponseDto>>> GetAllAsync(
    Expression<Func<MatchPrediction, bool>>? filter = null,
    Func<IQueryable<MatchPrediction>, IQueryable<MatchPrediction>>? include = null,
    Func<IQueryable<MatchPrediction>, IOrderedQueryable<MatchPrediction>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<MatchPredictionResponseDto>> GetByIdAsync(
    Guid id,
    Guid currentUserId,
    string userRole,
    Func<IQueryable<MatchPrediction>, IQueryable<MatchPrediction>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<MatchPredictionResponseDto>> GetMineByMatchIdAsync(
    Guid matchId,
    Guid currentUserId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPredictionPreviewDto>>> GetMineByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<UserSeasonPredictionPointsDto>> GetMySeasonPointsAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPredictionResponseDto>>> GetByMatchIdAsync(
    Guid matchId,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<List<MatchPredictionPreviewDto>>> GetByUserIdAsync(
    Guid userId,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedMatchPredictionResponseDto>> AddAsync(
    CreateMatchPredictionRequest request,
    Guid currentUserId,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateMatchPredictionRequest request,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default);
}
