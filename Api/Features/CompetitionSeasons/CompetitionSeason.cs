using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Competitions;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeason : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionSeason()
  {
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
}
