using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;

namespace Api.Features.CompetitionTeams;

// Responses
public sealed record CompetitionTeamResponseDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public CompetitionSeasonPreviewDto CompetitionSeason { get; init; } = default!;
  public Guid TeamId { get; init; }
  public TeamPreviewDto Team { get; init; } = default!;
  public Guid? CompetitionStageId { get; init; }
  public CompetitionStagePreviewDto? CompetitionStage { get; init; }
  public Guid? CompetitionGroupId { get; init; }
  public CompetitionGroupPreviewDto? CompetitionGroup { get; init; }
  public int? Seed { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CompetitionTeamPreviewDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public Guid TeamId { get; init; }
  public TeamPreviewDto Team { get; init; } = default!;
  public Guid? CompetitionStageId { get; init; }
  public Guid? CompetitionGroupId { get; init; }
  public int? Seed { get; init; }
  public bool IsActive { get; init; }
}

public sealed record CreatedCompetitionTeamResponseDto
{
  public Guid Id { get; init; }
  public Guid TeamId { get; init; }
}

// Requests
public sealed record CreateCompetitionTeamRequest(
  Guid CompetitionSeasonId,
  Guid TeamId,
  Guid? CompetitionStageId,
  Guid? CompetitionGroupId,
  int? Seed,
  bool IsActive = true);

public sealed record UpdateCompetitionTeamRequest(
  Guid Id,
  Guid CompetitionSeasonId,
  Guid TeamId,
  Guid? CompetitionStageId,
  Guid? CompetitionGroupId,
  int? Seed,
  bool IsActive);

public sealed record ChangeCompetitionTeamStatusRequest(
  Guid Id,
  bool IsActive);
