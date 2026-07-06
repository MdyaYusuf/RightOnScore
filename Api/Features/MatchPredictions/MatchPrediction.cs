using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Matches;
using Api.Features.Teams;
using Api.Features.Users;

namespace Api.Features.MatchPredictions;

public class MatchPrediction : Entity<Guid>
{
  [SetsRequiredMembers]
  public MatchPrediction()
  {
    User = default!;
    Match = default!;
  }

  public Guid UserId { get; set; }
  public Guid MatchId { get; set; }
  public int PredictedHomeScore { get; set; }
  public int PredictedAwayScore { get; set; }
  public Guid? PredictedAdvancingTeamId { get; set; }
  public int? PointsEarned { get; set; }

  // Relationship properties
  public virtual User User { get; set; }
  public virtual Match Match { get; set; }
  public virtual Team? PredictedAdvancingTeam { get; set; }
}
