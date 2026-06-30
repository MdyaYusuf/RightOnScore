using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;
using Riok.Mapperly.Abstractions;

namespace Api.Features.CompetitionTeams;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CompetitionTeamMapper
{
  public partial CompetitionTeam CreateToEntity(CreateCompetitionTeamRequest request);
  public partial void UpdateEntityFromRequest(UpdateCompetitionTeamRequest request, CompetitionTeam entity);
  public partial CompetitionTeamResponseDto EntityToResponseDto(CompetitionTeam entity);
  public partial CompetitionTeamPreviewDto EntityToPreviewDto(CompetitionTeam entity);
  public partial CreatedCompetitionTeamResponseDto EntityToCreatedResponseDto(CompetitionTeam entity);
  public partial CompetitionSeasonPreviewDto CompetitionSeasonToPreviewDto(CompetitionSeason entity);
  public partial TeamPreviewDto TeamToPreviewDto(Team entity);
  public partial CompetitionStagePreviewDto CompetitionStageToPreviewDto(CompetitionStage entity);
  public partial CompetitionGroupPreviewDto CompetitionGroupToPreviewDto(CompetitionGroup entity);
  public partial List<CompetitionTeamResponseDto> EntityToResponseDtoList(List<CompetitionTeam> entities);
  public partial List<CompetitionTeamPreviewDto> EntityToPreviewDtoList(List<CompetitionTeam> entities);
}
