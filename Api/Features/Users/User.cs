using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Roles;

namespace Api.Features.Users;

public class User : Entity<Guid>
{
  [SetsRequiredMembers]
  public User()
  {
    Username = default!;
    Email = default!;
    PasswordHash = default!;
    PasswordKey = default!;
  }

  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public required string PasswordKey { get; set; }
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpiration { get; set; }
  public string? ProfileImageUrl { get; set; }
  public bool IsActive { get; set; } = true;

  // Relationship properties
  public int RoleId { get; set; }
  public virtual Role Role { get; set; } = default!;
}