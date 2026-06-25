namespace Api.Features.Roles;

// Responses
public sealed record RoleResponseDto
{
  public int Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateRoleRequest(string Name);
public sealed record UpdateRoleRequest(int Id, string Name);