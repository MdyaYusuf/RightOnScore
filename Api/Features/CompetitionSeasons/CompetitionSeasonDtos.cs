using Api.Features.Competitions;

namespace Api.Features.CompetitionSeasons;

// Responses
public sealed record CompetitionSeasonResponseDto
{
  public Guid Id { get; init; }
  public Guid CompetitionId { get; init; }
  public CompetitionPreviewDto Competition { get; init; } = default!;
  public string Name { get; init; } = default!;
  public DateTime StartDate { get; init; }
  public DateTime EndDate { get; init; }
  public CompetitionSeasonStatus Status { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CompetitionSeasonPreviewDto
{
  public Guid Id { get; init; }
  public Guid CompetitionId { get; init; }
  public string Name { get; init; } = default!;
  public CompetitionSeasonStatus Status { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CreatedCompetitionSeasonResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateCompetitionSeasonRequest(
  Guid CompetitionId,
  string Name,
  DateTime StartDate,
  DateTime EndDate,
  CompetitionSeasonStatus Status,
  bool IsActive = false);

public sealed record UpdateCompetitionSeasonRequest(
  Guid Id,
  Guid CompetitionId,
  string Name,
  DateTime StartDate,
  DateTime EndDate,
  CompetitionSeasonStatus Status,
  bool IsActive);

public sealed record ChangeCompetitionSeasonStatusRequest(
  Guid Id,
  CompetitionSeasonStatus Status,
  bool IsActive);
