namespace Api.Features.Competitions;

// Responses
public sealed record CompetitionResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
  public string Country { get; init; } = default!;
  public string? LogoUrl { get; init; }
  public CompetitionType Type { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CompetitionPreviewDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
  public string? LogoUrl { get; init; }
  public CompetitionType Type { get; init; }
}

public sealed record CreatedCompetitionResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateCompetitionRequest(string Name, string Country, CompetitionType Type, string? LogoUrl, bool IsActive = true);
public sealed record UpdateCompetitionRequest(Guid Id, string Name, string Country, CompetitionType Type, string? LogoUrl, bool IsActive);
public sealed record ChangeCompetitionStatusRequest(Guid Id, bool IsActive);
