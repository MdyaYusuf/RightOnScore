using Api.Features.CompetitionStages;
using Riok.Mapperly.Abstractions;

namespace Api.Features.CompetitionGroups;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CompetitionGroupMapper
{
  public partial CompetitionGroup CreateToEntity(CreateCompetitionGroupRequest request);
  public partial void UpdateEntityFromRequest(UpdateCompetitionGroupRequest request, CompetitionGroup entity);
  public partial CompetitionGroupResponseDto EntityToResponseDto(CompetitionGroup entity);
  public partial CompetitionGroupPreviewDto EntityToPreviewDto(CompetitionGroup entity);
  public partial CreatedCompetitionGroupResponseDto EntityToCreatedResponseDto(CompetitionGroup entity);
  public partial CompetitionStagePreviewDto CompetitionStageToPreviewDto(CompetitionStage entity);
  public partial List<CompetitionGroupResponseDto> EntityToResponseDtoList(List<CompetitionGroup> entities);
  public partial List<CompetitionGroupPreviewDto> EntityToPreviewDtoList(List<CompetitionGroup> entities);
}
