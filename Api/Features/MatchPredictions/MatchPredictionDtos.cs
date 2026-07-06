using Api.Features.Matches;
using Api.Features.Users;

namespace Api.Features.MatchPredictions;

// Responses
public sealed record MatchPredictionResponseDto
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public UserPreviewDto User { get; init; } = default!;
  public Guid MatchId { get; init; }
  public MatchPreviewDto Match { get; init; } = default!;
  public int PredictedHomeScore { get; init; }
  public int PredictedAwayScore { get; init; }
  public Guid? PredictedAdvancingTeamId { get; init; }
  public int? PointsEarned { get; init; }
}

public sealed record MatchPredictionPreviewDto
{
  public Guid Id { get; init; }
  public Guid MatchId { get; init; }
  public MatchPreviewDto Match { get; init; } = default!;
  public int PredictedHomeScore { get; init; }
  public int PredictedAwayScore { get; init; }
  public Guid? PredictedAdvancingTeamId { get; init; }
  public int? PointsEarned { get; init; }
}

public sealed record CreatedMatchPredictionResponseDto
{
  public Guid Id { get; init; }
  public Guid MatchId { get; init; }
}

public sealed record UserSeasonPredictionPointsDto
{
  public Guid CompetitionSeasonId { get; init; }
  public int TotalPoints { get; init; }
  public int ScoredPredictionCount { get; init; }
}

// Requests
public sealed record CreateMatchPredictionRequest(
  Guid MatchId,
  int PredictedHomeScore,
  int PredictedAwayScore,
  Guid? PredictedAdvancingTeamId);

public sealed record UpdateMatchPredictionRequest(
  Guid Id,
  int PredictedHomeScore,
  int PredictedAwayScore,
  Guid? PredictedAdvancingTeamId);
