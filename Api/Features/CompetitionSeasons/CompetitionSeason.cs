using System.Diagnostics.CodeAnalysis;
using Api.Core.Entities;
using Api.Features.Competitions;
using Api.Features.CompetitionStages;
using Api.Features.CompetitionTeams;
using Api.Features.Matches;
using Api.Features.SeasonStandings;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeason : Entity<Guid>
{
  [SetsRequiredMembers]
  public CompetitionSeason()
  {
    CompetitionTeams = new HashSet<CompetitionTeam>();
    Matches = new HashSet<Match>();
    Stages = new HashSet<CompetitionStage>();
    SeasonStandings = new HashSet<SeasonStanding>();

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
  public virtual ICollection<Match> Matches { get; set; }
  public virtual ICollection<SeasonStanding> SeasonStandings { get; set; }
}
