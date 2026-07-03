using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;
using Riok.Mapperly.Abstractions;

namespace Api.Features.Matches;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class MatchMapper
{
  public partial Match CreateToEntity(CreateMatchRequest request);
  public partial void UpdateEntityFromRequest(UpdateMatchRequest request, Match entity);
  public partial MatchResponseDto EntityToResponseDto(Match entity);
  public partial MatchPreviewDto EntityToPreviewDto(Match entity);
  public partial CreatedMatchResponseDto EntityToCreatedResponseDto(Match entity);
  public partial CompetitionSeasonPreviewDto CompetitionSeasonToPreviewDto(CompetitionSeason entity);
  public partial CompetitionStagePreviewDto CompetitionStageToPreviewDto(CompetitionStage entity);
  public partial CompetitionGroupPreviewDto CompetitionGroupToPreviewDto(CompetitionGroup entity);
  public partial TeamPreviewDto TeamToPreviewDto(Team entity);
  public partial List<MatchResponseDto> EntityToResponseDtoList(List<Match> entities);
  public partial List<MatchPreviewDto> EntityToPreviewDtoList(List<Match> entities);
}
