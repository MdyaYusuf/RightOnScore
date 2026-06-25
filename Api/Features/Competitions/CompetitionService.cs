using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using FluentValidation;

namespace Api.Features.Competitions;

public class CompetitionService(
  ICompetitionRepository _competitionRepository,
  CompetitionMapper _mapper,
  CompetitionBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateCompetitionRequest> _createValidator,
  IValidator<UpdateCompetitionRequest> _updateValidator,
  IValidator<ChangeCompetitionStatusRequest> _changeStatusValidator) : ICompetitionService
{
  public async Task<ReturnModel<PagedResponse<CompetitionResponseDto>>> GetAllAsync(
    Expression<Func<Competition, bool>>? filter = null,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    Func<IQueryable<Competition>, IOrderedQueryable<Competition>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (competitions, totalCount) = await _competitionRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<CompetitionResponseDto> responseDtos = _mapper.EntityToResponseDtoList(competitions);
    var pagedResponse = new PagedResponse<CompetitionResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<CompetitionResponseDto>>()
    {
      Success = true,
      Message = "Yarışma listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<CompetitionPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
  {
    List<Competition> competitions = await _competitionRepository.GetActiveCompetitionsAsync(cancellationToken);
    List<CompetitionPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(competitions);

    return new ReturnModel<List<CompetitionPreviewDto>>()
    {
      Success = true,
      Message = "Aktif yarışmalar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionResponseDto>> GetAsync(
    Expression<Func<Competition, bool>> predicate,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Competition? competition = await _competitionRepository.GetAsync(predicate, include, enableTracking, cancellationToken);

    if (competition == null)
    {
      return new ReturnModel<CompetitionResponseDto>()
      {
        Success = true,
        Message = "Eşleşen yarışma bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    CompetitionResponseDto response = _mapper.EntityToResponseDto(competition);

    return new ReturnModel<CompetitionResponseDto>()
    {
      Success = true,
      Message = "Yarışma başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CompetitionResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Competition competition = await _businessRules.GetCompetitionIfExistAsync(id, include, enableTracking, cancellationToken);
    CompetitionResponseDto response = _mapper.EntityToResponseDto(competition);

    return new ReturnModel<CompetitionResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı yarışma başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCompetitionResponseDto>> AddAsync(
    CreateCompetitionRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    await _businessRules.NameMustBeUniqueAsync(request.Name, cancellationToken);

    Competition competition = _mapper.CreateToEntity(request);

    await _competitionRepository.AddAsync(competition, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedCompetitionResponseDto response = _mapper.EntityToCreatedResponseDto(competition);

    return new ReturnModel<CreatedCompetitionResponseDto>()
    {
      Success = true,
      Message = "Yarışma başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCompetitionRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Competition competition = await _businessRules.GetCompetitionIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);
    await _businessRules.CompetitionNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.Name, cancellationToken);

    _mapper.UpdateEntityFromRequest(request, competition);

    _competitionRepository.Update(competition);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeCompetitionStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Competition competition = await _businessRules.GetCompetitionIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    competition.IsActive = request.IsActive;

    _competitionRepository.Update(competition);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = request.IsActive ? "Yarışma aktif hale getirildi." : "Yarışma pasif hale getirildi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    Competition competition = await _businessRules.GetCompetitionIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _competitionRepository.Delete(competition);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Yarışma başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}
