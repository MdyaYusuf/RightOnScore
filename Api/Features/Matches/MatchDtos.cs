using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;

namespace Api.Features.Matches;

// Responses
public sealed record MatchResponseDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public CompetitionSeasonPreviewDto CompetitionSeason { get; init; } = default!;
  public Guid? CompetitionStageId { get; init; }
  public CompetitionStagePreviewDto? CompetitionStage { get; init; }
  public Guid? CompetitionGroupId { get; init; }
  public CompetitionGroupPreviewDto? CompetitionGroup { get; init; }
  public Guid HomeTeamId { get; init; }
  public TeamPreviewDto HomeTeam { get; init; } = default!;
  public Guid AwayTeamId { get; init; }
  public TeamPreviewDto AwayTeam { get; init; } = default!;
  public DateTime KickoffTime { get; init; }
  public MatchStatus Status { get; init; }
  public int? Round { get; init; }
  public string? Venue { get; init; }
  public int? HomeScore { get; init; }
  public int? AwayScore { get; init; }
  public Guid? AdvancingTeamId { get; init; }
}

public sealed record MatchPreviewDto
{
  public Guid Id { get; init; }
  public Guid CompetitionSeasonId { get; init; }
  public Guid HomeTeamId { get; init; }
  public TeamPreviewDto HomeTeam { get; init; } = default!;
  public Guid AwayTeamId { get; init; }
  public TeamPreviewDto AwayTeam { get; init; } = default!;
  public DateTime KickoffTime { get; init; }
  public MatchStatus Status { get; init; }
  public int? HomeScore { get; init; }
  public int? AwayScore { get; init; }
  public Guid? AdvancingTeamId { get; init; }
}

public sealed record CreatedMatchResponseDto
{
  public Guid Id { get; init; }
  public DateTime KickoffTime { get; init; }
}

// Requests
public sealed record CreateMatchRequest(
  Guid CompetitionSeasonId,
  Guid? CompetitionStageId,
  Guid? CompetitionGroupId,
  Guid HomeTeamId,
  Guid AwayTeamId,
  DateTime KickoffTime,
  int? Round,
  string? Venue,
  MatchStatus Status = MatchStatus.Scheduled);

public sealed record UpdateMatchRequest(
  Guid Id,
  Guid CompetitionSeasonId,
  Guid? CompetitionStageId,
  Guid? CompetitionGroupId,
  Guid HomeTeamId,
  Guid AwayTeamId,
  DateTime KickoffTime,
  int? Round,
  string? Venue);

public sealed record ChangeMatchStatusRequest(
  Guid Id,
  MatchStatus Status);

public sealed record RecordMatchResultRequest(
  Guid Id,
  int HomeScore,
  int AwayScore,
  Guid? AdvancingTeamId);
