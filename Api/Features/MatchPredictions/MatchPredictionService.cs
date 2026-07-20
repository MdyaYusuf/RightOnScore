using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using Api.Features.Matches;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.MatchPredictions;

public class MatchPredictionService(
  IMatchPredictionRepository _matchPredictionRepository,
  MatchPredictionMapper _mapper,
  MatchPredictionBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateMatchPredictionRequest> _createValidator,
  IValidator<UpdateMatchPredictionRequest> _updateValidator) : IMatchPredictionService
{
  public async Task<ReturnModel<PagedResponse<MatchPredictionResponseDto>>> GetAllAsync(
    Expression<Func<MatchPrediction, bool>>? filter = null,
    Func<IQueryable<MatchPrediction>, IQueryable<MatchPrediction>>? include = null,
    Func<IQueryable<MatchPrediction>, IOrderedQueryable<MatchPrediction>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (predictions, totalCount) = await _matchPredictionRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? IncludeDetails,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<MatchPredictionResponseDto> responseDtos = _mapper.EntityToResponseDtoList(predictions);
    var pagedResponse = new PagedResponse<MatchPredictionResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<MatchPredictionResponseDto>>()
    {
      Success = true,
      Message = "Skor tahmini listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MatchPredictionResponseDto>> GetByIdAsync(
    Guid id,
    Guid currentUserId,
    string userRole,
    Func<IQueryable<MatchPrediction>, IQueryable<MatchPrediction>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    MatchPrediction prediction = await _businessRules.GetMatchPredictionIfExistAsync(
      id,
      include: include ?? IncludeDetails,
      enableTracking,
      cancellationToken);

    Match match = prediction.Match
      ?? await _businessRules.MatchMustExistAsync(prediction.MatchId, cancellationToken: cancellationToken);

    _businessRules.EnsureCanViewPredictionScores(
      match,
      prediction.UserId,
      currentUserId,
      userRole);

    MatchPredictionResponseDto response = _mapper.EntityToResponseDto(prediction);

    return new ReturnModel<MatchPredictionResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı skor tahmini başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MatchPredictionResponseDto>> GetMineByMatchIdAsync(
    Guid matchId,
    Guid currentUserId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.MatchMustExistAsync(matchId, cancellationToken: cancellationToken);

    MatchPrediction? prediction = await _matchPredictionRepository.GetByUserIdAndMatchIdAsync(
      currentUserId,
      matchId,
      cancellationToken);

    if (prediction == null)
    {
      return new ReturnModel<MatchPredictionResponseDto>()
      {
        Success = true,
        Message = "Bu maç için skor tahmininiz bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    MatchPredictionResponseDto response = _mapper.EntityToResponseDto(prediction);

    return new ReturnModel<MatchPredictionResponseDto>()
    {
      Success = true,
      Message = "Maç için skor tahmininiz başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPredictionPreviewDto>>> GetMineByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);
    await _businessRules.UserMustExistAndBeActiveAsync(currentUserId, cancellationToken);

    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByCompetitionSeasonIdAndUserIdAsync(
      competitionSeasonId,
      currentUserId,
      cancellationToken);

    List<MatchPredictionPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(predictions);

    return new ReturnModel<List<MatchPredictionPreviewDto>>()
    {
      Success = true,
      Message = "Sezondaki skor tahminleriniz başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<UserSeasonPredictionPointsDto>> GetMySeasonPointsAsync(
    Guid competitionSeasonId,
    Guid currentUserId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);
    await _businessRules.UserMustExistAndBeActiveAsync(currentUserId, cancellationToken);

    int totalPoints = await _matchPredictionRepository.GetTotalPointsByUserAndCompetitionSeasonIdAsync(
      currentUserId,
      competitionSeasonId,
      cancellationToken);

    int scoredCount = await _matchPredictionRepository.GetScoredPredictionCountByUserAndCompetitionSeasonIdAsync(
      currentUserId,
      competitionSeasonId,
      cancellationToken);

    var response = new UserSeasonPredictionPointsDto
    {
      CompetitionSeasonId = competitionSeasonId,
      TotalPoints = totalPoints,
      ScoredPredictionCount = scoredCount
    };

    return new ReturnModel<UserSeasonPredictionPointsDto>()
    {
      Success = true,
      Message = "Sezon puan özeti başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPredictionResponseDto>>> GetByMatchIdAsync(
    Guid matchId,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);
    await _businessRules.MatchMustExistAsync(matchId, cancellationToken: cancellationToken);

    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByMatchIdAsync(matchId, cancellationToken);
    List<MatchPredictionResponseDto> responseDtos = _mapper.EntityToResponseDtoList(predictions);

    return new ReturnModel<List<MatchPredictionResponseDto>>()
    {
      Success = true,
      Message = "Maça ait skor tahminleri başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MatchPredictionsRevealResponseDto>> GetRevealedByMatchIdAsync(
    Guid matchId,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.UserMustExistAndBeActiveAsync(currentUserId, cancellationToken);
    Match match = await _businessRules.MatchMustExistAsync(matchId, cancellationToken: cancellationToken);

    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByMatchIdAsync(matchId, cancellationToken);
    bool areRevealed = _businessRules.ArePredictionsRevealed(match) || userRole == "Admin";

    var response = new MatchPredictionsRevealResponseDto
    {
      MatchId = matchId,
      AreRevealed = areRevealed,
      PredictionCount = predictions.Count,
      Predictions = areRevealed
        ? _mapper.EntityToRevealItemDtoList(predictions)
        : []
    };

    string message = areRevealed
      ? "Maça ait skor tahminleri başarılı bir şekilde getirildi."
      : "Tahminler maç başlayana kadar gizlidir. Katılım sayısı döndürüldü.";

    return new ReturnModel<MatchPredictionsRevealResponseDto>()
    {
      Success = true,
      Message = message,
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPredictionPreviewDto>>> GetByUserIdAsync(
    Guid userId,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.UserMustExistAndBeActiveAsync(userId, cancellationToken);
    await _businessRules.UserMustExistAndBeActiveAsync(currentUserId, cancellationToken);

    List<MatchPrediction> predictions = await _matchPredictionRepository.GetByUserIdAsync(userId, cancellationToken);

    bool isSelfOrAdmin = userId == currentUserId || userRole == "Admin";

    if (!isSelfOrAdmin)
    {
      predictions = predictions
        .Where(prediction =>
          prediction.Match != null
          && _businessRules.ArePredictionsRevealed(prediction.Match))
        .ToList();
    }

    List<MatchPredictionPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(predictions);

    return new ReturnModel<List<MatchPredictionPreviewDto>>()
    {
      Success = true,
      Message = "Kullanıcıya ait skor tahminleri başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedMatchPredictionResponseDto>> AddAsync(
    CreateMatchPredictionRequest request,
    Guid currentUserId,
    CancellationToken cancellationToken = default)
  {
    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    await _businessRules.UserMustExistAndBeActiveAsync(currentUserId, cancellationToken);
    _businessRules.PredictedScoresMustBeValid(request.PredictedHomeScore, request.PredictedAwayScore);
    await _businessRules.UserMustNotHavePredictionForMatchAsync(currentUserId, request.MatchId, cancellationToken);

    Match match = await _businessRules.MatchMustExistAsync(
      request.MatchId,
      include: query => query.Include(m => m.CompetitionStage),
      cancellationToken: cancellationToken);
    _businessRules.MatchMustBeOpenForPredictions(match);
    _businessRules.KnockoutPredictionMustBeValid(
      match,
      request.PredictedHomeScore,
      request.PredictedAwayScore,
      request.PredictedAdvancingTeamId);

    MatchPrediction prediction = _mapper.CreateToEntity(request);
    prediction.UserId = currentUserId;

    await _matchPredictionRepository.AddAsync(prediction, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedMatchPredictionResponseDto response = _mapper.EntityToCreatedResponseDto(prediction);

    return new ReturnModel<CreatedMatchPredictionResponseDto>()
    {
      Success = true,
      Message = "Skor tahmini başarılı bir şekilde oluşturuldu.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateMatchPredictionRequest request,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    MatchPrediction prediction = await _businessRules.GetMatchPredictionIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _businessRules.UserMustOwnPredictionOrBeAdmin(prediction, currentUserId, userRole);
    _businessRules.PredictedScoresMustBeValid(request.PredictedHomeScore, request.PredictedAwayScore);

    Match match = await _businessRules.MatchMustExistAsync(
      prediction.MatchId,
      include: query => query.Include(m => m.CompetitionStage),
      cancellationToken: cancellationToken);
    _businessRules.MatchMustBeOpenForPredictions(match);
    _businessRules.PredictionCanOnlyBeModifiedBeforeKickoff(match);
    _businessRules.KnockoutPredictionMustBeValid(
      match,
      request.PredictedHomeScore,
      request.PredictedAwayScore,
      request.PredictedAdvancingTeamId);

    _mapper.UpdateEntityFromRequest(request, prediction);

    _matchPredictionRepository.Update(prediction);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Skor tahmini başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    Guid currentUserId,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    MatchPrediction prediction = await _businessRules.GetMatchPredictionIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _businessRules.UserMustOwnPredictionOrBeAdmin(prediction, currentUserId, userRole);

    Match match = await _businessRules.MatchMustExistAsync(prediction.MatchId, cancellationToken: cancellationToken);
    _businessRules.PredictionCanOnlyBeModifiedBeforeKickoff(match);

    _matchPredictionRepository.Delete(prediction);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Skor tahmini başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }

  private static IQueryable<MatchPrediction> IncludeDetails(IQueryable<MatchPrediction> query)
  {
    return query
      .Include(p => p.User)
        .ThenInclude(u => u.Role)
      .Include(p => p.Match)
        .ThenInclude(m => m.HomeTeam)
      .Include(p => p.Match)
        .ThenInclude(m => m.AwayTeam);
  }
}
