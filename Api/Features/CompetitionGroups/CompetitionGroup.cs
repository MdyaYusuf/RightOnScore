using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionStages;
using Api.Features.CompetitionTeams;

namespace Api.Features.CompetitionGroups;

public class CompetitionGroup : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionGroup()
  {
    CompetitionTeams = new HashSet<CompetitionTeam>();

    Name = default!;
    CompetitionStage = default!;
  }

  public Guid CompetitionStageId { get; set; }
  public required string Name { get; set; }
  public int DisplayOrder { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual CompetitionStage CompetitionStage { get; set; }
  public virtual ICollection<CompetitionTeam> CompetitionTeams { get; set; }
}
