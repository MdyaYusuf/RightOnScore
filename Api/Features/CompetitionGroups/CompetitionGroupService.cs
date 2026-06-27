using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using Api.Features.CompetitionStages;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionGroups;

public class CompetitionGroupService(
  ICompetitionGroupRepository _competitionGroupRepository,
  CompetitionGroupMapper _mapper,
  CompetitionGroupBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateCompetitionGroupRequest> _createValidator,
  IValidator<UpdateCompetitionGroupRequest> _updateValidator,
  IValidator<ChangeCompetitionGroupStatusRequest> _changeStatusValidator) : ICompetitionGroupService
{
  public async Task<ReturnModel<PagedResponse<CompetitionGroupResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionGroup, bool>>? filter = null,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    Func<IQueryable<CompetitionGroup>, IOrderedQueryable<CompetitionGroup>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (competitionGroups, totalCount) = await _competitionGroupRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? (query => query.Include(cg => cg.CompetitionStage)),
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<CompetitionGroupResponseDto> responseDtos = _mapper.EntityToResponseDtoList(competitionGroups);
    var pagedResponse = new PagedResponse<CompetitionGroupResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<CompetitionGroupResponseDto>>()
    {
      Success = true,
      Message = "Yarışma grubu listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionGroupPreviewDto>>> GetByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionStageMustExistAsync(competitionStageId, cancellationToken);

    List<CompetitionGroup> competitionGroups = await _competitionGroupRepository.GetByCompetitionStageIdAsync(competitionStageId, cancellationToken);
    List<CompetitionGroupPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionGroups);

    return new ReturnModel<List<CompetitionGroupPreviewDto>>()
    {
      Success = true,
      Message = "Aşamaya ait yarışma grupları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionGroupPreviewDto>>> GetActiveByCompetitionStageIdAsync(
    Guid competitionStageId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionStageMustExistAsync(competitionStageId, cancellationToken);

    List<CompetitionGroup> competitionGroups = await _competitionGroupRepository.GetActiveByCompetitionStageIdAsync(competitionStageId, cancellationToken);
    List<CompetitionGroupPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionGroups);

    return new ReturnModel<List<CompetitionGroupPreviewDto>>()
    {
      Success = true,
      Message = "Aşamaya ait aktif yarışma grupları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionGroupResponseDto>> GetAsync(
    Expression<Func<CompetitionGroup, bool>> predicate,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionGroup? competitionGroup = await _competitionGroupRepository.GetAsync(
      predicate,
      include: include ?? (query => query.Include(cg => cg.CompetitionStage)),
      enableTracking,
      cancellationToken);

    if (competitionGroup == null)
    {
      return new ReturnModel<CompetitionGroupResponseDto>()
      {
        Success = true,
        Message = "Eşleşen yarışma grubu bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionGroupResponseDto response = _mapper.EntityToResponseDto(competitionGroup);

    return new ReturnModel<CompetitionGroupResponseDto>()
    {
      Success = true,
      Message = "Yarışma grubu başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionGroupResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionGroup competitionGroup = await _businessRules.GetCompetitionGroupIfExistAsync(
      id,
      include: include ?? (query => query.Include(cg => cg.CompetitionStage)),
      enableTracking,
      cancellationToken);

    CompetitionGroupResponseDto response = _mapper.EntityToResponseDto(competitionGroup);

    return new ReturnModel<CompetitionGroupResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı yarışma grubu başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCompetitionGroupResponseDto>> AddAsync(
    CreateCompetitionGroupRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionStage competitionStage = await _businessRules.CompetitionStageMustExistAsync(request.CompetitionStageId, cancellationToken);
    _businessRules.CompetitionStageMustBeGroupStage(competitionStage);
    _businessRules.ActiveGroupRequiresActiveStage(competitionStage, request.IsActive);

    await _businessRules.NameMustBeUniqueForStageAsync(request.CompetitionStageId, request.Name, cancellationToken);
    await _businessRules.DisplayOrderMustBeUniqueForStageAsync(request.CompetitionStageId, request.DisplayOrder, cancellationToken);

    CompetitionGroup competitionGroup = _mapper.CreateToEntity(request);

    await _competitionGroupRepository.AddAsync(competitionGroup, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedCompetitionGroupResponseDto response = _mapper.EntityToCreatedResponseDto(competitionGroup);

    return new ReturnModel<CreatedCompetitionGroupResponseDto>()
    {
      Success = true,
      Message = "Yarışma grubu başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionGroupRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionGroup competitionGroup = await _businessRules.GetCompetitionGroupIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    CompetitionStage competitionStage = await _businessRules.CompetitionStageMustExistAsync(request.CompetitionStageId, cancellationToken);
    _businessRules.CompetitionStageMustBeGroupStage(competitionStage);
    _businessRules.ActiveGroupRequiresActiveStage(competitionStage, request.IsActive);

    await _businessRules.GroupNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionStageId, request.Name, cancellationToken);
    await _businessRules.DisplayOrderCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionStageId, request.DisplayOrder, cancellationToken);

    _mapper.UpdateEntityFromRequest(request, competitionGroup);

    _competitionGroupRepository.Update(competitionGroup);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma grubu başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionGroupStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    CompetitionGroup competitionGroup = await _businessRules.GetCompetitionGroupIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    if (request.IsActive)
    {
      CompetitionStage competitionStage = await _businessRules.CompetitionStageMustExistAsync(competitionGroup.CompetitionStageId, cancellationToken);
      _businessRules.CompetitionStageMustBeGroupStage(competitionStage);
      _businessRules.ActiveGroupRequiresActiveStage(competitionStage, request.IsActive);
    }

    competitionGroup.IsActive = request.IsActive;

    _competitionGroupRepository.Update(competitionGroup);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = request.IsActive ? "Yarışma grubu aktif hale getirildi." : "Yarışma grubu pasif hale getirildi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    CompetitionGroup competitionGroup = await _businessRules.GetCompetitionGroupIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _competitionGroupRepository.Delete(competitionGroup);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma grubu başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}
