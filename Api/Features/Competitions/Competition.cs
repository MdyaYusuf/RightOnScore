using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;

namespace Api.Features.Competitions;

public class Competition : Entity<Guid>
{
  [SetsRequiredMembers]
  public Competition()
  {
    Name = default!;
    Country = default!;
  }

  public required string Name { get; set; }
  public required string Country { get; set; }
  public string? LogoUrl { get; set; }
  public CompetitionType Type { get; set; }
  public bool IsActive { get; set; }
}
