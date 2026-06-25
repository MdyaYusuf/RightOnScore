using Api.Features.Authentication;
using Riok.Mapperly.Abstractions;

namespace Api.Features.Users;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMapper
{
  [MapperIgnoreSource(nameof(RegisterUserRequest.Password))]
  [MapperIgnoreTarget(nameof(User.PasswordHash))]
  [MapperIgnoreTarget(nameof(User.PasswordKey))]
  public partial User CreateToEntity(RegisterUserRequest request);
  [MapperIgnoreTarget(nameof(User.ProfileImageUrl))]
  public partial void UpdateEntityFromRequest(UpdateUserRequest request, User entity);
  [MapProperty("Role.Name", nameof(UserResponseDto.RoleName))]
  public partial UserResponseDto EntityToResponseDto(User entity);
  public partial CreatedUserResponseDto EntityToCreatedResponseDto(User entity);
  public partial List<UserResponseDto> EntityToResponseDtoList(List<User> entities);
  [MapProperty("Role.Name", "RoleName")]
  public partial UserPreviewDto EntityToPreviewDto(User entity);
  public partial List<UserPreviewDto> EntityToPreviewDtoList(List<User> entities);
}