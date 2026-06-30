using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;

namespace Api.Features.CompetitionTeams;

public class CompetitionTeam : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionTeam()
  {
    CompetitionSeason = default!;
    Team = default!;
  }

  public Guid CompetitionSeasonId { get; set; }
  public Guid TeamId { get; set; }
  public Guid? CompetitionStageId { get; set; }
  public Guid? CompetitionGroupId { get; set; }
  public int? Seed { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual CompetitionSeason CompetitionSeason { get; set; }
  public virtual Team Team { get; set; }
  public virtual CompetitionStage? CompetitionStage { get; set; }
  public virtual CompetitionGroup? CompetitionGroup { get; set; }
}
