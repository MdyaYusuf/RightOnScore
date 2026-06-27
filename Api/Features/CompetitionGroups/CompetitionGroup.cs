using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionStages;

namespace Api.Features.CompetitionGroups;

public class CompetitionGroup : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionGroup()
  {
    Name = default!;
    CompetitionStage = default!;
  }

  public Guid CompetitionStageId { get; set; }
  public required string Name { get; set; }
  public int DisplayOrder { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual CompetitionStage CompetitionStage { get; set; }
}
