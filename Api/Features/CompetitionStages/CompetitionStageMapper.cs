using Api.Features.CompetitionSeasons;
using Riok.Mapperly.Abstractions;

namespace Api.Features.CompetitionStages;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CompetitionStageMapper
{
  public partial CompetitionStage CreateToEntity(CreateCompetitionStageRequest request);
  public partial void UpdateEntityFromRequest(UpdateCompetitionStageRequest request, CompetitionStage entity);
  public partial CompetitionStageResponseDto EntityToResponseDto(CompetitionStage entity);
  public partial CompetitionStagePreviewDto EntityToPreviewDto(CompetitionStage entity);
  public partial CreatedCompetitionStageResponseDto EntityToCreatedResponseDto(CompetitionStage entity);
  public partial CompetitionSeasonPreviewDto CompetitionSeasonToPreviewDto(CompetitionSeason entity);
  public partial List<CompetitionStageResponseDto> EntityToResponseDtoList(List<CompetitionStage> entities);
  public partial List<CompetitionStagePreviewDto> EntityToPreviewDtoList(List<CompetitionStage> entities);
}
