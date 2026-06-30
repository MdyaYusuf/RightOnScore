using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionTeams;

public class CompetitionTeamService(
  ICompetitionTeamRepository _competitionTeamRepository,
  CompetitionTeamMapper _mapper,
  CompetitionTeamBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateCompetitionTeamRequest> _createValidator,
  IValidator<UpdateCompetitionTeamRequest> _updateValidator,
  IValidator<ChangeCompetitionTeamStatusRequest> _changeStatusValidator) : ICompetitionTeamService
{
  public async Task<ReturnModel<PagedResponse<CompetitionTeamResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionTeam, bool>>? filter = null,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    Func<IQueryable<CompetitionTeam>, IOrderedQueryable<CompetitionTeam>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (competitionTeams, totalCount) = await _competitionTeamRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? IncludeDetails,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<CompetitionTeamResponseDto> responseDtos = _mapper.EntityToResponseDtoList(competitionTeams);
    var pagedResponse = new PagedResponse<CompetitionTeamResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<CompetitionTeamResponseDto>>()
    {
      Success = true,
      Message = "Yarışma takımı listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<CompetitionTeam> competitionTeams = await _competitionTeamRepository.GetByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<CompetitionTeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionTeams);

    return new ReturnModel<List<CompetitionTeamPreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetActiveByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<CompetitionTeam> competitionTeams = await _competitionTeamRepository.GetActiveByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<CompetitionTeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionTeams);

    return new ReturnModel<List<CompetitionTeamPreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait aktif takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionStageMustExistAsync(competitionStageId, cancellationToken);

    List<CompetitionTeam> competitionTeams = await _competitionTeamRepository.GetByCompetitionStageIdAsync(competitionStageId, cancellationToken);
    List<CompetitionTeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionTeams);

    return new ReturnModel<List<CompetitionTeamPreviewDto>>()
    {
      Success = true,
      Message = "Aşamaya ait takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionTeamPreviewDto>>> GetByCompetitionGroupIdAsync(
    Guid competitionGroupId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionGroupMustExistAsync(competitionGroupId, cancellationToken);

    List<CompetitionTeam> competitionTeams = await _competitionTeamRepository.GetByCompetitionGroupIdAsync(competitionGroupId, cancellationToken);
    List<CompetitionTeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionTeams);

    return new ReturnModel<List<CompetitionTeamPreviewDto>>()
    {
      Success = true,
      Message = "Gruba ait takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionTeamResponseDto>> GetAsync(
    Expression<Func<CompetitionTeam, bool>> predicate,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionTeam? competitionTeam = await _competitionTeamRepository.GetAsync(
      predicate,
      include: include ?? IncludeDetails,
      enableTracking,
      cancellationToken);

    if (competitionTeam == null)
    {
      return new ReturnModel<CompetitionTeamResponseDto>()
      {
        Success = true,
        Message = "Eşleşen yarışma takımı bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionTeamResponseDto response = _mapper.EntityToResponseDto(competitionTeam);

    return new ReturnModel<CompetitionTeamResponseDto>()
    {
      Success = true,
      Message = "Yarışma takımı başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionTeamResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionTeam competitionTeam = await _businessRules.GetCompetitionTeamIfExistAsync(
      id,
      include: include ?? IncludeDetails,
      enableTracking,
      cancellationToken);

    CompetitionTeamResponseDto response = _mapper.EntityToResponseDto(competitionTeam);

    return new ReturnModel<CompetitionTeamResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı yarışma takımı başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCompetitionTeamResponseDto>> AddAsync(
    CreateCompetitionTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionSeason competitionSeason = await _businessRules.CompetitionSeasonMustExistAsync(request.CompetitionSeasonId, cancellationToken);
    Team team = await _businessRules.TeamMustExistAsync(request.TeamId, cancellationToken);
    _businessRules.TeamMustBeActive(team);

    CompetitionStage? competitionStage = await _businessRules.CompetitionStageMustBelongToSeasonAsync(request.CompetitionSeasonId, request.CompetitionStageId, cancellationToken);
    CompetitionGroup? competitionGroup = await _businessRules.CompetitionGroupMustBelongToStageAsync(competitionStage, request.CompetitionGroupId, cancellationToken);

    _businessRules.GroupAssignmentMustUseGroupStage(competitionStage, competitionGroup);
    _businessRules.ActiveMembershipRequiresActiveParents(request.IsActive, competitionSeason, competitionStage, competitionGroup);

    await _businessRules.TeamMustBeUniqueInSeasonAsync(request.CompetitionSeasonId, request.TeamId, cancellationToken);

    CompetitionTeam competitionTeam = _mapper.CreateToEntity(request);

    await _competitionTeamRepository.AddAsync(competitionTeam, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedCompetitionTeamResponseDto response = _mapper.EntityToCreatedResponseDto(competitionTeam);

    return new ReturnModel<CreatedCompetitionTeamResponseDto>()
    {
      Success = true,
      Message = "Takım yarışmaya başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionTeam competitionTeam = await _businessRules.GetCompetitionTeamIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    CompetitionSeason competitionSeason = await _businessRules.CompetitionSeasonMustExistAsync(request.CompetitionSeasonId, cancellationToken);
    Team team = await _businessRules.TeamMustExistAsync(request.TeamId, cancellationToken);
    _businessRules.TeamMustBeActive(team);

    CompetitionStage? competitionStage = await _businessRules.CompetitionStageMustBelongToSeasonAsync(request.CompetitionSeasonId, request.CompetitionStageId, cancellationToken);
    CompetitionGroup? competitionGroup = await _businessRules.CompetitionGroupMustBelongToStageAsync(competitionStage, request.CompetitionGroupId, cancellationToken);

    _businessRules.GroupAssignmentMustUseGroupStage(competitionStage, competitionGroup);
    _businessRules.ActiveMembershipRequiresActiveParents(request.IsActive, competitionSeason, competitionStage, competitionGroup);

    await _businessRules.TeamCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionSeasonId, request.TeamId, cancellationToken);

    _mapper.UpdateEntityFromRequest(request, competitionTeam);

    _competitionTeamRepository.Update(competitionTeam);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma takımı başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionTeamStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionTeam competitionTeam = await _businessRules.GetCompetitionTeamIfExistAsync(
      request.Id,
      include: IncludeDetails,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (request.IsActive)
    {
      _businessRules.ActiveMembershipRequiresActiveParents(
        request.IsActive,
        competitionTeam.CompetitionSeason,
        competitionTeam.CompetitionStage,
        competitionTeam.CompetitionGroup);
    }

    competitionTeam.IsActive = request.IsActive;

    _competitionTeamRepository.Update(competitionTeam);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = request.IsActive ? "Yarışma takımı aktif hale getirildi." : "Yarışma takımı pasif hale getirildi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    CompetitionTeam competitionTeam = await _businessRules.GetCompetitionTeamIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _competitionTeamRepository.Delete(competitionTeam);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma takımı başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }

  private static IQueryable<CompetitionTeam> IncludeDetails(IQueryable<CompetitionTeam> query)
  {
    return query
      .Include(ct => ct.CompetitionSeason)
      .Include(ct => ct.Team)
      .Include(ct => ct.CompetitionStage)
      .Include(ct => ct.CompetitionGroup);
  }
}
