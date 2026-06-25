using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeasonService(
  ICompetitionSeasonRepository _competitionSeasonRepository,
  CompetitionSeasonMapper _mapper,
  CompetitionSeasonBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateCompetitionSeasonRequest> _createValidator,
  IValidator<UpdateCompetitionSeasonRequest> _updateValidator,
  IValidator<ChangeCompetitionSeasonStatusRequest> _changeStatusValidator) : ICompetitionSeasonService
{
  public async Task<ReturnModel<PagedResponse<CompetitionSeasonResponseDto>>> GetAllAsync(
    Expression<Func<CompetitionSeason, bool>>? filter = null,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    Func<IQueryable<CompetitionSeason>, IOrderedQueryable<CompetitionSeason>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (competitionSeasons, totalCount) = await _competitionSeasonRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include: include ?? (query => query.Include(cs => cs.Competition)),
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<CompetitionSeasonResponseDto> responseDtos = _mapper.EntityToResponseDtoList(competitionSeasons);
    var pagedResponse = new PagedResponse<CompetitionSeasonResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<CompetitionSeasonResponseDto>>()
    {
      Success = true,
      Message = "Yarışma sezonu listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionSeasonPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
  {
    List<CompetitionSeason> competitionSeasons = await _competitionSeasonRepository.GetActiveSeasonsAsync(cancellationToken);
    List<CompetitionSeasonPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionSeasons);

    return new ReturnModel<List<CompetitionSeasonPreviewDto>>()
    {
      Success = true,
      Message = "Aktif yarışma sezonları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionSeasonPreviewDto>>> GetByCompetitionIdAsync(
    Guid competitionId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionMustExistAsync(competitionId, cancellationToken);

    List<CompetitionSeason> competitionSeasons = await _competitionSeasonRepository.GetByCompetitionIdAsync(competitionId, cancellationToken);
    List<CompetitionSeasonPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitionSeasons);

    return new ReturnModel<List<CompetitionSeasonPreviewDto>>()
    {
      Success = true,
      Message = "Yarışmaya ait sezonlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionSeasonResponseDto>> GetActiveByCompetitionIdAsync(
    Guid competitionId,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.CompetitionMustExistAsync(competitionId, cancellationToken);

    CompetitionSeason? competitionSeason = await _competitionSeasonRepository.GetActiveSeasonByCompetitionIdAsync(competitionId, cancellationToken);

    if (competitionSeason == null)
    {
      return new ReturnModel<CompetitionSeasonResponseDto>()
      {
        Success = true,
        Message = "Yarışmaya ait aktif sezon bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionSeasonResponseDto response = _mapper.EntityToResponseDto(competitionSeason);

    return new ReturnModel<CompetitionSeasonResponseDto>()
    {
      Success = true,
      Message = "Yarışmaya ait aktif sezon başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionSeasonResponseDto>> GetAsync(
    Expression<Func<CompetitionSeason, bool>> predicate,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionSeason? competitionSeason = await _competitionSeasonRepository.GetAsync(
      predicate,
      include: include ?? (query => query.Include(cs => cs.Competition)),
      enableTracking,
      cancellationToken);

    if (competitionSeason == null)
    {
      return new ReturnModel<CompetitionSeasonResponseDto>()
      {
        Success = true,
        Message = "Eşleşen yarışma sezonu bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionSeasonResponseDto response = _mapper.EntityToResponseDto(competitionSeason);

    return new ReturnModel<CompetitionSeasonResponseDto>()
    {
      Success = true,
      Message = "Yarışma sezonu başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionSeasonResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    CompetitionSeason competitionSeason = await _businessRules.GetCompetitionSeasonIfExistAsync(
      id,
      include: include ?? (query => query.Include(cs => cs.Competition)),
      enableTracking,
      cancellationToken);

    CompetitionSeasonResponseDto response = _mapper.EntityToResponseDto(competitionSeason);

    return new ReturnModel<CompetitionSeasonResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı yarışma sezonu başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCompetitionSeasonResponseDto>> AddAsync(
    CreateCompetitionSeasonRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.SeasonDateRangeMustBeValid(request.StartDate, request.EndDate);
    _businessRules.ActiveSeasonMustHaveActiveStatus(request.IsActive, request.Status);

    await _businessRules.CompetitionMustExistAsync(request.CompetitionId, cancellationToken);
    await _businessRules.NameMustBeUniqueForCompetitionAsync(request.CompetitionId, request.Name, cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionMustBeActiveAsync(request.CompetitionId, cancellationToken);
      await _businessRules.OnlyOneActiveSeasonPerCompetitionAsync(request.CompetitionId, cancellationToken: cancellationToken);
    }

    CompetitionSeason competitionSeason = _mapper.CreateToEntity(request);

    await _competitionSeasonRepository.AddAsync(competitionSeason, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedCompetitionSeasonResponseDto response = _mapper.EntityToCreatedResponseDto(competitionSeason);

    return new ReturnModel<CreatedCompetitionSeasonResponseDto>()
    {
      Success = true,
      Message = "Yarışma sezonu başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionSeasonRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.SeasonDateRangeMustBeValid(request.StartDate, request.EndDate);
    _businessRules.ActiveSeasonMustHaveActiveStatus(request.IsActive, request.Status);

    CompetitionSeason competitionSeason = await _businessRules.GetCompetitionSeasonIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    await _businessRules.CompetitionMustExistAsync(request.CompetitionId, cancellationToken);
    await _businessRules.SeasonNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.CompetitionId, request.Name, cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionMustBeActiveAsync(request.CompetitionId, cancellationToken);
      await _businessRules.OnlyOneActiveSeasonPerCompetitionAsync(request.CompetitionId, request.Id, cancellationToken);
    }

    _mapper.UpdateEntityFromRequest(request, competitionSeason);

    _competitionSeasonRepository.Update(competitionSeason);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma sezonu başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionSeasonStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.ActiveSeasonMustHaveActiveStatus(request.IsActive, request.Status);

    CompetitionSeason competitionSeason = await _businessRules.GetCompetitionSeasonIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    if (request.IsActive)
    {
      await _businessRules.CompetitionMustBeActiveAsync(competitionSeason.CompetitionId, cancellationToken);
      await _businessRules.OnlyOneActiveSeasonPerCompetitionAsync(competitionSeason.CompetitionId, request.Id, cancellationToken);
    }

    competitionSeason.Status = request.Status;
    competitionSeason.IsActive = request.IsActive;

    _competitionSeasonRepository.Update(competitionSeason);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma sezonu durumu başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    CompetitionSeason competitionSeason = await _businessRules.GetCompetitionSeasonIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _competitionSeasonRepository.Delete(competitionSeason);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma sezonu başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}
