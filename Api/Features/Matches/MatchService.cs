using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.MatchPredictions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Matches;

public class MatchService(
  IMatchRepository _matchRepository,
  MatchMapper _mapper,
  MatchBusinessRules _businessRules,
  IMatchPredictionScoringService _matchPredictionScoringService,
  IUnitOfWork _unitOfWork,
  IValidator<CreateMatchRequest> _createValidator,
  IValidator<UpdateMatchRequest> _updateValidator,
  IValidator<ChangeMatchStatusRequest> _changeStatusValidator,
  IValidator<RecordMatchResultRequest> _recordResultValidator) : IMatchService
{
  public async Task<ReturnModel<PagedResponse<MatchResponseDto>>> GetAllAsync(
    Expression<Func<Match, bool>>? filter = null,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    Func<IQueryable<Match>, IOrderedQueryable<Match>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (matches, totalCount) = await _matchRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? IncludeDetails,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<MatchResponseDto> responseDtos = _mapper.EntityToResponseDtoList(matches);
    var pagedResponse = new PagedResponse<MatchResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<MatchResponseDto>>()
    {
      Success = true,
      Message = "Maç listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<Match> matches = await _matchRepository.GetByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<MatchPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(matches);

    return new ReturnModel<List<MatchPreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait maçlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPreviewDto>>> GetUpcomingByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<Match> matches = await _matchRepository.GetUpcomingByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<MatchPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(matches);

    return new ReturnModel<List<MatchPreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait yaklaşan maçlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionStageMustExistAsync(competitionStageId, cancellationToken);

    List<Match> matches = await _matchRepository.GetByCompetitionStageIdAsync(competitionStageId, cancellationToken);
    List<MatchPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(matches);

    return new ReturnModel<List<MatchPreviewDto>>()
    {
      Success = true,
      Message = "Aşamaya ait maçlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<MatchPreviewDto>>> GetByCompetitionGroupIdAsync(
    Guid competitionGroupId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionGroupMustExistAsync(competitionGroupId, cancellationToken);

    List<Match> matches = await _matchRepository.GetByCompetitionGroupIdAsync(competitionGroupId, cancellationToken);
    List<MatchPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(matches);

    return new ReturnModel<List<MatchPreviewDto>>()
    {
      Success = true,
      Message = "Gruba ait maçlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MatchResponseDto>> GetAsync(
    Expression<Func<Match, bool>> predicate,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Match? match = await _matchRepository.GetAsync(
      predicate,
      include: include ?? IncludeDetails,
      enableTracking,
      cancellationToken);

    if (match == null)
    {
      return new ReturnModel<MatchResponseDto>()
      {
        Success = true,
        Message = "Eşleşen maç bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    MatchResponseDto response = _mapper.EntityToResponseDto(match);

    return new ReturnModel<MatchResponseDto>()
    {
      Success = true,
      Message = "Maç başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<MatchResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Match match = await _businessRules.GetMatchIfExistAsync(
      id,
      include: include ?? IncludeDetails,
      enableTracking,
      cancellationToken);

    MatchResponseDto response = _mapper.EntityToResponseDto(match);

    return new ReturnModel<MatchResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı maç başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedMatchResponseDto>> AddAsync(
    CreateMatchRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    await ValidateFixtureAsync(request.CompetitionSeasonId, request.CompetitionStageId, request.CompetitionGroupId, request.HomeTeamId, request.AwayTeamId, request.KickoffTime, cancellationToken);

    Match match = _mapper.CreateToEntity(request);

    await _matchRepository.AddAsync(match, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedMatchResponseDto response = _mapper.EntityToCreatedResponseDto(match);

    return new ReturnModel<CreatedMatchResponseDto>()
    {
      Success = true,
      Message = "Maç başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateMatchRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Match match = await _businessRules.GetMatchIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);
    _businessRules.FixtureCanOnlyBeModifiedWhenScheduledOrPostponed(match);

    await ValidateFixtureAsync(request.CompetitionSeasonId, request.CompetitionStageId, request.CompetitionGroupId, request.HomeTeamId, request.AwayTeamId, request.KickoffTime, cancellationToken);

    _mapper.UpdateEntityFromRequest(request, match);

    _matchRepository.Update(match);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Maç başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeMatchStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Match match = await _businessRules.GetMatchIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    match.Status = request.Status;

    _matchRepository.Update(match);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Maç durumu başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RecordResultAsync(
    RecordMatchResultRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _recordResultValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Match match = await _businessRules.GetMatchIfExistAsync(
      request.Id,
      include: query => query.Include(m => m.CompetitionStage),
      enableTracking: true,
      cancellationToken: cancellationToken);

    _businessRules.ResultCanOnlyBeRecordedForLiveOrScheduledMatch(match);
    _businessRules.ScoresMustBeValid(request.HomeScore, request.AwayScore);
    _businessRules.AdvancingTeamMustBeValidForRecordedResult(
      match.CompetitionStage,
      request.HomeScore,
      request.AwayScore,
      request.AdvancingTeamId,
      match.HomeTeamId,
      match.AwayTeamId);

    match.HomeScore = request.HomeScore;
    match.AwayScore = request.AwayScore;
    match.AdvancingTeamId = request.HomeScore == request.AwayScore
      ? request.AdvancingTeamId
      : null;
    match.Status = MatchStatus.Finished;

    _matchRepository.Update(match);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await _matchPredictionScoringService.ScoreMatchAsync(match.Id, cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Maç sonucu başarılı bir şekilde kaydedildi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> CorrectResultAsync(
    RecordMatchResultRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _recordResultValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Match match = await _businessRules.GetMatchIfExistAsync(
      request.Id,
      include: query => query.Include(m => m.CompetitionStage),
      enableTracking: true,
      cancellationToken: cancellationToken);

    _businessRules.ResultCanOnlyBeCorrectedWhenFinished(match);
    _businessRules.ScoresMustBeValid(request.HomeScore, request.AwayScore);
    _businessRules.AdvancingTeamMustBeValidForRecordedResult(
      match.CompetitionStage,
      request.HomeScore,
      request.AwayScore,
      request.AdvancingTeamId,
      match.HomeTeamId,
      match.AwayTeamId);

    match.HomeScore = request.HomeScore;
    match.AwayScore = request.AwayScore;
    match.AdvancingTeamId = request.HomeScore == request.AwayScore
      ? request.AdvancingTeamId
      : null;

    _matchRepository.Update(match);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await _matchPredictionScoringService.RescoreMatchAndLaterAsync(match.Id, cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Maç sonucu düzeltildi. İlgili tahminler yeniden puanlandı.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    Match match = await _businessRules.GetMatchIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _matchRepository.Delete(match);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Maç başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }

  private async Task ValidateFixtureAsync(
    Guid competitionSeasonId,
    Guid? competitionStageId,
    Guid? competitionGroupId,
    Guid homeTeamId,
    Guid awayTeamId,
    DateTime kickoffTime,
    CancellationToken cancellationToken)
  {
    _businessRules.HomeAndAwayTeamsMustBeDifferent(homeTeamId, awayTeamId);

    CompetitionSeason competitionSeason = await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);
    await _businessRules.TeamMustExistAsync(homeTeamId, cancellationToken);
    await _businessRules.TeamMustExistAsync(awayTeamId, cancellationToken);
    await _businessRules.TeamsMustBeRegisteredInSeasonAsync(competitionSeasonId, homeTeamId, awayTeamId, cancellationToken);

    CompetitionStage? competitionStage = await _businessRules.CompetitionStageMustBelongToSeasonAsync(competitionSeasonId, competitionStageId, cancellationToken);
    CompetitionGroup? competitionGroup = await _businessRules.CompetitionGroupMustBelongToStageAsync(competitionStage, competitionGroupId, cancellationToken);

    _businessRules.GroupAssignmentMustUseGroupStage(competitionStage, competitionGroup);
    _businessRules.KickoffMustBeWithinSeason(competitionSeason, kickoffTime);
    _businessRules.KickoffMustBeWithinStage(competitionStage, kickoffTime);
  }

  private static IQueryable<Match> IncludeDetails(IQueryable<Match> query)
  {
    return query
      .Include(m => m.CompetitionSeason)
      .Include(m => m.CompetitionStage)
      .Include(m => m.CompetitionGroup)
      .Include(m => m.HomeTeam)
      .Include(m => m.AwayTeam);
  }
}
