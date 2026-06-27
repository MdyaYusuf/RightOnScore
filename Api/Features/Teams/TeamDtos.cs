namespace Api.Features.Teams;

// Responses
public sealed record TeamResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
  public string ShortName { get; init; } = default!;
  public string Country { get; init; } = default!;
  public string? CrestUrl { get; init; }
  public bool IsActive { get; init; }
}

public sealed record TeamPreviewDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
  public string ShortName { get; init; } = default!;
  public string? CrestUrl { get; init; }
}

public sealed record CreatedTeamResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateTeamRequest(
  string Name,
  string ShortName,
  string Country,
  IFormFile? CrestFile,
  bool IsActive = true);

public sealed record UpdateTeamRequest(
  Guid Id,
  string Name,
  string ShortName,
  string Country,
  IFormFile? CrestFile,
  bool IsActive);

public sealed record ChangeTeamStatusRequest(
  Guid Id,
  bool IsActive);

public sealed record SearchTeamRequest(string SearchTerm);
