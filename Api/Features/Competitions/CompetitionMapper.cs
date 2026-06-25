using Riok.Mapperly.Abstractions;

namespace Api.Features.Competitions;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CompetitionMapper
{
  public partial Competition CreateToEntity(CreateCompetitionRequest request);
  public partial void UpdateEntityFromRequest(UpdateCompetitionRequest request, Competition entity);
  public partial CompetitionResponseDto EntityToResponseDto(Competition entity);
  public partial CompetitionPreviewDto EntityToPreviewDto(Competition entity);
  public partial CreatedCompetitionResponseDto EntityToCreatedResponseDto(Competition entity);
  public partial List<CompetitionResponseDto> EntityToResponseDtoList(List<Competition> entities);
  public partial List<CompetitionPreviewDto> EntityToPreviewDtoList(List<Competition> entities);
}
