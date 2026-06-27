using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;

namespace Api.Features.Teams;

public class Team : Entity<Guid>
{
  [SetsRequiredMembers]
  public Team()
  {
    Name = default!;
    ShortName = default!;
    Country = default!;
  }

  public required string Name { get; set; }
  public required string ShortName { get; set; }
  public required string Country { get; set; }
  public string? CrestUrl { get; set; }
  public bool IsActive { get; set; }
}
