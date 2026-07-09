using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionSeasons;
using Api.Features.Users;

namespace Api.Features.SeasonStandings;

public class SeasonStanding : Entity<Guid>
{
  [SetsRequiredMembers]
  public SeasonStanding()
  {
    User = default!;
    CompetitionSeason = default!;
  }

  public Guid UserId { get; set; }
  public Guid CompetitionSeasonId { get; set; }
  public int TotalPoints { get; set; }
  public int ExactScoreCount { get; set; }
  public int ScoredPredictionCount { get; set; }

  // Relationship properties
  public virtual User User { get; set; }
  public virtual CompetitionSeason CompetitionSeason { get; set; }
}
