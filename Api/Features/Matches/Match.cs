using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.MatchPredictions;
using Api.Features.Teams;

namespace Api.Features.Matches;

public class Match : Entity<Guid>
{
  [SetsRequiredMembers]
  public Match()
  {
    CompetitionSeason = default!;
    HomeTeam = default!;
    AwayTeam = default!;
    MatchPredictions = new HashSet<MatchPrediction>();
  }

  public Guid CompetitionSeasonId { get; set; }
  public Guid? CompetitionStageId { get; set; }
  public Guid? CompetitionGroupId { get; set; }
  public Guid HomeTeamId { get; set; }
  public Guid AwayTeamId { get; set; }
  public DateTime KickoffTime { get; set; }
  public MatchStatus Status { get; set; }
  public int? Round { get; set; }
  public string? Venue { get; set; }
  public int? HomeScore { get; set; }
  public int? AwayScore { get; set; }
  public Guid? AdvancingTeamId { get; set; }

  // Relationship properties
  public virtual CompetitionSeason CompetitionSeason { get; set; }
  public virtual CompetitionStage? CompetitionStage { get; set; }
  public virtual CompetitionGroup? CompetitionGroup { get; set; }
  public virtual Team HomeTeam { get; set; }
  public virtual Team AwayTeam { get; set; }
  public virtual Team? AdvancingTeam { get; set; }
  public virtual ICollection<MatchPrediction> MatchPredictions { get; set; }
}
