using System.Linq.Expressions;
using Api.Core.Helpers;
using Api.Core.Repositories;
using Api.Core.Responses;
using FluentValidation;

namespace Api.Features.Teams;

public class TeamService(
  ITeamRepository _teamRepository,
  TeamMapper _mapper,
  TeamBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateTeamRequest> _createValidator,
  IValidator<UpdateTeamRequest> _updateValidator,
  IValidator<ChangeTeamStatusRequest> _changeStatusValidator,
  IValidator<SearchTeamRequest> _searchValidator) : ITeamService
{
  public async Task<ReturnModel<PagedResponse<TeamResponseDto>>> GetAllAsync(
    Expression<Func<Team, bool>>? filter = null,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    Func<IQueryable<Team>, IOrderedQueryable<Team>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (teams, totalCount) = await _teamRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<TeamResponseDto> responseDtos = _mapper.EntityToResponseDtoList(teams);
    var pagedResponse = new PagedResponse<TeamResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<TeamResponseDto>>()
    {
      Success = true,
      Message = "Takım listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<TeamPreviewDto>>> GetActiveAsync(CancellationToken cancellationToken = default)
  {
    List<Team> teams = await _teamRepository.GetActiveTeamsAsync(cancellationToken);
    List<TeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(teams);

    return new ReturnModel<List<TeamPreviewDto>>()
    {
      Success = true,
      Message = "Aktif takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<TeamPreviewDto>>> GetByCountryAsync(
    string country,
    CancellationToken cancellationToken = default)
  {
    List<Team> teams = await _teamRepository.GetByCountryAsync(country, cancellationToken);
    List<TeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(teams);

    return new ReturnModel<List<TeamPreviewDto>>()
    {
      Success = true,
      Message = "Ülkeye ait takımlar başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<List<TeamPreviewDto>>> SearchAsync(
    SearchTeamRequest request,
    CancellationToken cancellationToken = default)
  {
    var validationResult = await _searchValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    List<Team> teams = await _teamRepository.SearchByNameAsync(request.SearchTerm, cancellationToken);
    List<TeamPreviewDto> responseDtos = _mapper.EntityToPreviewDtoList(teams);

    return new ReturnModel<List<TeamPreviewDto>>()
    {
      Success = true,
      Message = "Takım arama sonuçları başarılı bir şekilde getirildi.",
      Data = responseDtos,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<TeamResponseDto>> GetAsync(
    Expression<Func<Team, bool>> predicate,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Team? team = await _teamRepository.GetAsync(predicate, include, enableTracking, cancellationToken);

    if (team == null)
    {
      return new ReturnModel<TeamResponseDto>()
      {
        Success = true,
        Message = "Eşleşen takım bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    TeamResponseDto response = _mapper.EntityToResponseDto(team);

    return new ReturnModel<TeamResponseDto>()
    {
      Success = true,
      Message = "Takım başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<TeamResponseDto>> GetByIdAsync(
    Guid id,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Team team = await _businessRules.GetTeamIfExistAsync(id, include, enableTracking, cancellationToken);
    TeamResponseDto response = _mapper.EntityToResponseDto(team);

    return new ReturnModel<TeamResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı takım başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedTeamResponseDto>> AddAsync(
    CreateTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    await _businessRules.NameMustBeUniqueForCountryAsync(request.Name, request.Country, cancellationToken);
    await _businessRules.ShortNameMustBeUniqueForCountryAsync(request.ShortName, request.Country, cancellationToken);

    Team team = _mapper.CreateToEntity(request);
    team.CrestUrl = await FileHelper.ReplaceImageOnDisk(
      request.CrestFile,
      team.CrestUrl,
      "teams",
      request.Name,
      cancellationToken);

    await _teamRepository.AddAsync(team, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    CreatedTeamResponseDto response = _mapper.EntityToCreatedResponseDto(team);

    return new ReturnModel<CreatedTeamResponseDto>()
    {
      Success = true,
      Message = "Takım başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateTeamRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Team team = await _businessRules.GetTeamIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    await _businessRules.TeamNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.Name, request.Country, cancellationToken);
    await _businessRules.TeamShortNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.ShortName, request.Country, cancellationToken);

    team.CrestUrl = await FileHelper.ReplaceImageOnDisk(
      request.CrestFile,
      team.CrestUrl,
      "teams",
      request.Name,
      cancellationToken);

    _mapper.UpdateEntityFromRequest(request, team);

    _teamRepository.Update(team);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Takım başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> ChangeStatusAsync(
    ChangeTeamStatusRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _changeStatusValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Team team = await _businessRules.GetTeamIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    team.IsActive = request.IsActive;

    _teamRepository.Update(team);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = request.IsActive ? "Takım aktif hale getirildi." : "Takım pasif hale getirildi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    Team team = await _businessRules.GetTeamIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _teamRepository.Delete(team);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Takım başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}
