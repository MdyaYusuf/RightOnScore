using Api.Features.CompetitionSeasons;

namespace Api.Features.CompetitionStages;

// Responses
public sealed record CompetitionStageResponseDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public CompetitionSeasonPreviewDto CompetitionSeason { get; init; } = default!;
  public string Name { get; init; } = default!;
  public CompetitionStageType Type { get; init; }
  public int DisplayOrder { get; init; }
  public DateTime StartDate { get; init; }
  public DateTime EndDate { get; init; }
  public CompetitionStageStatus Status { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CompetitionStagePreviewDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public string Name { get; init; } = default!;
  public CompetitionStageType Type { get; init; }
  public int DisplayOrder { get; init; }
  public CompetitionStageStatus Status { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CreatedCompetitionStageResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateCompetitionStageRequest(
  Guid CompetitionSeasonId,
  string Name,
  CompetitionStageType Type,
  int DisplayOrder,
  DateTime StartDate,
  DateTime EndDate,
  CompetitionStageStatus Status,
  bool IsActive = false);

public sealed record UpdateCompetitionStageRequest(
  Guid Id,
  Guid CompetitionSeasonId,
  string Name,
  CompetitionStageType Type,
  int DisplayOrder,
  DateTime StartDate,
  DateTime EndDate,
  CompetitionStageStatus Status,
  bool IsActive);

public sealed record ChangeCompetitionStageStatusRequest(
  Guid Id,
  CompetitionStageStatus Status,
  bool IsActive);
