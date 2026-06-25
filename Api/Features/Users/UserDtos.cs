namespace Api.Features.Users;

// Responses
public class UserResponseDto
{
  public Guid Id { get; set; }
  public string Username { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string? ProfileImageUrl { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedDate { get; set; }
  public int RoleId { get; set; }
  public string RoleName { get; set; } = null!;
}

public sealed record UserPreviewDto
{
  public Guid Id { get; init; }
  public string Username { get; init; } = null!;
  public string? ProfileImageUrl { get; init; }
  public string RoleName { get; init; } = null!;
}

public sealed record CreatedUserResponseDto
{
  public Guid Id { get; init; }
  public string Username { get; init; } = null!;
}

// Requests
public sealed record UpdateUserRequest(
  string Username,
  string Email,
  IFormFile? ImageFile);

public sealed record ChangePasswordRequest(
  string CurrentPassword,
  string NewPassword,
  string ConfirmNewPassword);