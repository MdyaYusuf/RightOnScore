using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using Api.Features.CompetitionSeasons;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionStages;

public class CompetitionStageService(
  ICompetitionStageRepository _competitionStageRepository,
  CompetitionStageMapper _mapper,
  CompetitionStageBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateCompetitionStageRequest> _createValidator,
  IValidator<UpdateCompetitionStageRequest> _updateValidator,
  IValidator<ChangeCompetitionStageStatusRequest> _changeStatusValidator) : ICompetitionStageService
{
  public async Task<ReturnModel<PagedResponse<CompetitionStageResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionStage, bool>>? filter = null,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    Func<IQueryable<CompetitionStage>, IOrderedQueryable<CompetitionStage>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (competitionStages, totalCount) = await _competitionStageRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? (query => query.Include(cs => cs.CompetitionSeason)),
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<CompetitionStageResponseDto> responseDtos = _mapper.EntityToResponseDtoList(competitionStages);
    var pagedResponse = new PagedResponse<CompetitionStageResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<CompetitionStageResponseDto>>()
    {
      Success = true,
      Message = "Yarışma aşaması listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionStagePreviewDto>>> GetByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<CompetitionStage> competitionStages = await _competitionStageRepository.GetByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<CompetitionStagePreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionStages);

    return new ReturnModel<List<CompetitionStagePreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait yarışma aşamaları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionStagePreviewDto>>> GetActiveByCompetitionSeasonIdAsync(
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    List<CompetitionStage> competitionStages = await _competitionStageRepository.GetActiveByCompetitionSeasonIdAsync(competitionSeasonId, cancellationToken);
    List<CompetitionStagePreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionStages);

    return new ReturnModel<List<CompetitionStagePreviewDto>>()
    {
      Success = true,
      Message = "Sezona ait aktif yarışma aşamaları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionStageResponseDto>> GetAsync(
    Expression<Func<CompetitionStage, bool>> predicate,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionStage? competitionStage = await _competitionStageRepository.GetAsync(
      predicate,
      include: include ?? (query => query.Include(cs => cs.CompetitionSeason)),
      enableTracking,
      cancellationToken);

    if (competitionStage == null)
    {
      return new ReturnModel<CompetitionStageResponseDto>()
      {
        Success = true,
        Message = "Eşleşen yarışma aşaması bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionStageResponseDto response = _mapper.EntityToResponseDto(competitionStage);

    return new ReturnModel<CompetitionStageResponseDto>()
    {
      Success = true,
      Message = "Yarışma aşaması başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionStageResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionStage competitionStage = await _businessRules.GetCompetitionStageIfExistAsync(
      id,
      include: include ?? (query => query.Include(cs => cs.CompetitionSeason)),
      enableTracking,
      cancellationToken);

    CompetitionStageResponseDto response = _mapper.EntityToResponseDto(competitionStage);

    return new ReturnModel<CompetitionStageResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı yarışma aşaması başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCompetitionStageResponseDto>> AddAsync(
    CreateCompetitionStageRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.StageDateRangeMustBeValid(request.StartDate, request.EndDate);
    _businessRules.ActiveStageMustHaveActiveStatus(request.IsActive, request.Status);

    CompetitionSeason competitionSeason = await _businessRules.CompetitionSeasonMustExistAsync(request.CompetitionSeasonId, cancellationToken);
    _businessRules.StageDateRangeMustBeInsideSeason(competitionSeason, request.StartDate, request.EndDate);

    await _businessRules.NameMustBeUniqueForSeasonAsync(request.CompetitionSeasonId, request.Name, cancellationToken);
    await _businessRules.DisplayOrderMustBeUniqueForSeasonAsync(request.CompetitionSeasonId, request.DisplayOrder, cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionSeasonMustBeActiveAsync(request.CompetitionSeasonId, cancellationToken);
    }

    CompetitionStage competitionStage = _mapper.CreateToEntity(request);

    await _competitionStageRepository.AddAsync(competitionStage, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedCompetitionStageResponseDto response = _mapper.EntityToCreatedResponseDto(competitionStage);

    return new ReturnModel<CreatedCompetitionStageResponseDto>()
    {
      Success = true,
      Message = "Yarışma aşaması başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionStageRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.StageDateRangeMustBeValid(request.StartDate, request.EndDate);
    _businessRules.ActiveStageMustHaveActiveStatus(request.IsActive, request.Status);

    CompetitionStage competitionStage = await _businessRules.GetCompetitionStageIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    CompetitionSeason competitionSeason = await _businessRules.CompetitionSeasonMustExistAsync(request.CompetitionSeasonId, cancellationToken);
    _businessRules.StageDateRangeMustBeInsideSeason(competitionSeason, request.StartDate, request.EndDate);

    await _businessRules.StageNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionSeasonId, request.Name, cancellationToken);
    await _businessRules.DisplayOrderCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionSeasonId, request.DisplayOrder, cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionSeasonMustBeActiveAsync(request.CompetitionSeasonId, cancellationToken);
    }

    _mapper.UpdateEntityFromRequest(request, competitionStage);

    _competitionStageRepository.Update(competitionStage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma aşaması başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionStageStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.ActiveStageMustHaveActiveStatus(request.IsActive, request.Status);

    CompetitionStage competitionStage = await _businessRules.GetCompetitionStageIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionSeasonMustBeActiveAsync(competitionStage.CompetitionSeasonId, cancellationToken);
    }

    competitionStage.Status = request.Status;
    competitionStage.IsActive = request.IsActive;

    _competitionStageRepository.Update(competitionStage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma aşaması durumu başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    CompetitionStage competitionStage = await _businessRules.GetCompetitionStageIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _competitionStageRepository.Delete(competitionStage);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma aşaması başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}
