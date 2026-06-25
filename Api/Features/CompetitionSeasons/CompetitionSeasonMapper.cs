using Api.Features.Competitions;
using Riok.Mapperly.Abstractions;

namespace Api.Features.CompetitionSeasons;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CompetitionSeasonMapper
{
  public partial CompetitionSeason CreateToEntity(CreateCompetitionSeasonRequest request);
  public partial void UpdateEntityFromRequest(UpdateCompetitionSeasonRequest request, CompetitionSeason entity);
  public partial CompetitionSeasonResponseDto EntityToResponseDto(CompetitionSeason entity);
  public partial CompetitionSeasonPreviewDto EntityToPreviewDto(CompetitionSeason entity);
  public partial CreatedCompetitionSeasonResponseDto EntityToCreatedResponseDto(CompetitionSeason entity);
  public partial CompetitionPreviewDto CompetitionToPreviewDto(Competition entity);
  public partial List<CompetitionSeasonResponseDto> EntityToResponseDtoList(List<CompetitionSeason> entities);
  public partial List<CompetitionSeasonPreviewDto> EntityToPreviewDtoList(List<CompetitionSeason> entities);
}
