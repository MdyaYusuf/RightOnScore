using Api.Features.Users;

namespace Api.Features.SeasonStandings;

// Responses
public sealed record SeasonStandingResponseDto
{
  public Guid Id { get; init; }
  public int Rank { get; init; }
  public Guid UserId { get; init; }
  public UserPreviewDto User { get; init; } = default!;
  public Guid CompetitionSeasonId { get; init; }
  public int TotalPoints { get; init; }
  public int ExactScoreCount { get; init; }
  public int ScoredPredictionCount { get; init; }
}

public sealed record SeasonStandingPreviewDto
{
  public int Rank { get; init; }
  public Guid UserId { get; init; }
  public UserPreviewDto User { get; init; } = default!;
  public int TotalPoints { get; init; }
  public int ExactScoreCount { get; init; }
  public int ScoredPredictionCount { get; init; }
}

public sealed record MySeasonStandingDto
{
  public Guid CompetitionSeasonId { get; init; }
  public int Rank { get; init; }
  public int TotalPoints { get; init; }
  public int ExactScoreCount { get; init; }
  public int ScoredPredictionCount { get; init; }
  public int TotalParticipants { get; init; }
}
