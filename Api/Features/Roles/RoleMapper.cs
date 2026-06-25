using Riok.Mapperly.Abstractions;

namespace Api.Features.Roles;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RoleMapper
{
  public partial Role CreateToEntity(CreateRoleRequest request);
  public partial void UpdateEntityFromRequest(UpdateRoleRequest request, Role entity);
  public partial RoleResponseDto EntityToResponseDto(Role entity);
  public partial List<RoleResponseDto> EntityToResponseDtoList(List<Role> entities);
}