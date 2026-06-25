using System.Linq.Expressions;
using Api.Core.Repositories;
using Api.Core.Responses;
using FluentValidation;

namespace Api.Features.Roles;

public class RoleService(
  IRoleRepository _roleRepository,
  RoleMapper _mapper,
  RoleBusinessRules _businessRules,
  IUnitOfWork _unitOfWork,
  IValidator<CreateRoleRequest> _createValidator,
  IValidator<UpdateRoleRequest> _updateValidator) : IRoleService
{
  public async Task<ReturnModel<PagedResponse<RoleResponseDto>>> GetAllAsync(
    string userRole,
    Expression<Func<Role, bool>>? filter = null,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    bool enableTracking = false,
    bool withDeleted = false,
    CancellationToken cancellationToken = default)
  {
    var (roles, totalCount) = await _roleRepository.GetPagedListAsync(
      pageNumber,
      pageSize,
      filter,
      include,
      orderBy,
      enableTracking,
      withDeleted,
      cancellationToken);

    List<RoleResponseDto> responseDtos = _mapper.EntityToResponseDtoList(roles);
    var pagedResponse = new PagedResponse<RoleResponseDto>(responseDtos, totalCount, pageNumber, pageSize);

    return new ReturnModel<PagedResponse<RoleResponseDto>>()
    {
      Success = true,
      Message = "Rol listesi başarılı bir şekilde getirildi.",
      Data = pagedResponse,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<RoleResponseDto>> GetAsync(
    Expression<Func<Role, bool>> predicate,
    string userRole,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Role? role = await _roleRepository.GetAsync(predicate, include, enableTracking, cancellationToken);

    if (role == null)
    {
      return new ReturnModel<RoleResponseDto>()
      {
        Success = true,
        Message = "Eşleşen rol bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    RoleResponseDto response = _mapper.EntityToResponseDto(role);

    return new ReturnModel<RoleResponseDto>
    {
      Success = true,
      Message = "Rol başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<RoleResponseDto>> GetByIdAsync(
    int id,
    string userRole,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    Role role = await _businessRules.GetRoleIfExistAsync(id, include, enableTracking, cancellationToken);

    RoleResponseDto response = _mapper.EntityToResponseDto(role);

    return new ReturnModel<RoleResponseDto>()
    {
      Success = true,
      Message = $"{id} numaralı rol başarılı bir şekilde getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<RoleResponseDto>> AddAsync(
    CreateRoleRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    _businessRules.AdminRoleRequired(userRole);

    await _businessRules.NameMustBeUniqueAsync(request.Name, cancellationToken);

    Role role = _mapper.CreateToEntity(request);

    await _roleRepository.AddAsync(role, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    RoleResponseDto response = _mapper.EntityToResponseDto(role);

    return new ReturnModel<RoleResponseDto>()
    {
      Success = true,
      Message = "Rol başarılı bir şekilde eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateRoleRequest request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _businessRules.AdminRoleRequired(userRole);

    var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    Role role = await _businessRules.GetRoleIfExistAsync(request.Id, enableTracking: true, cancellationToken: cancellationToken);

    _businessRules.CoreRolesCannotBeModifiedOrDeleted(role);
    await _businessRules.RoleNameCannotBeDuplicatedWhenUpdatedAsync(request.Id, request.Name, cancellationToken);

    _mapper.UpdateEntityFromRequest(request, role);

    _roleRepository.Update(role);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Rol başarılı bir şekilde güncellendi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    int id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    Role role = await _businessRules.GetRoleIfExistAsync(id, enableTracking: true, cancellationToken: cancellationToken);

    _businessRules.AdminRoleRequired(userRole);
    _businessRules.CoreRolesCannotBeModifiedOrDeleted(role);

    _roleRepository.Delete(role);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>()
    {
      Success = true,
      Message = "Rol başarılı bir şekilde silindi.",
      StatusCode = 200
    };
  }
}