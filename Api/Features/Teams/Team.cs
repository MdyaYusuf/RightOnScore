using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.CompetitionTeams;

namespace Api.Features.Teams;

public class Team : Entity<Guid>
{
  [SetsRequiredMembers]
  public Team()
  {
    CompetitionTeams = new HashSet<CompetitionTeam>();

    Name = default!;
    ShortName = default!;
    Country = default!;
  }

  public required string Name { get; set; }
  public required string ShortName { get; set; }
  public required string Country { get; set; }
  public string? CrestUrl { get; set; }
  public bool IsActive { get; set; }

  // Relationship properties
  public virtual ICollection<CompetitionTeam> CompetitionTeams { get; set; }
}
