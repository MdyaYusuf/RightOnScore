using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Competitions;
using Api.Features.CompetitionStages;
using Api.Features.CompetitionTeams;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeason : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionSeason()
  {
    CompetitionTeams = new HashSet<CompetitionTeam>();
    Stages = new HashSet<CompetitionStage>();

    Name = default!;
    Competition = default!;
  }

  public Guid CompetitionId { get; set; }
  public required string Name { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public CompetitionSeasonStatus Status { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual Competition Competition { get; set; }
  public virtual ICollection<CompetitionStage> Stages { get; set; }
  public virtual ICollection<CompetitionTeam> CompetitionTeams { get; set; }
}
