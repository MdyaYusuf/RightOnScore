using Api.Features.CompetitionStages;

namespace Api.Features.CompetitionGroups;

// Responses
public sealed record CompetitionGroupResponseDto
{
  public Guid Id { get; init; }
  public Guid CompetitionStageId { get; init; }
  public CompetitionStagePreviewDto CompetitionStage { get; init; } = default!;
  public string Name { get; init; } = default!;
  public int DisplayOrder { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CompetitionGroupPreviewDto
{
  public Guid Id { get; init; }
  public Guid CompetitionStageId { get; init; }
  public string Name { get; init; } = default!;
  public int DisplayOrder { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CreatedCompetitionGroupResponseDto
{
  public Guid Id { get; init; }
  public string Name { get; init; } = default!;
}

// Requests
public sealed record CreateCompetitionGroupRequest(
  Guid CompetitionStageId,
  string Name,
  int DisplayOrder,
  bool IsActive = true);

public sealed record UpdateCompetitionGroupRequest(
  Guid Id,
  Guid CompetitionStageId,
  string Name,
  int DisplayOrder,
  bool IsActive);

public sealed record ChangeCompetitionGroupStatusRequest(
  Guid Id,
  bool IsActive);
