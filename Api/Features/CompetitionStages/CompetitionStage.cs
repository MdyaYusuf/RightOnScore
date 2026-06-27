using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;

namespace Api.Features.CompetitionStages;

public class CompetitionStage : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionStage()
  {
    Groups = new HashSet<CompetitionGroup>();

    Name = default!;
    CompetitionSeason = default!;
  }

  public Guid CompetitionSeasonId { get; set; }
  public required string Name { get; set; }
  public CompetitionStageType Type { get; set; }
  public int DisplayOrder { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public CompetitionStageStatus Status { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual CompetitionSeason CompetitionSeason { get; set; }
  public virtual ICollection<CompetitionGroup> Groups { get; set; }
}
