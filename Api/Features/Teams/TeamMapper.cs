using Riok.Mapperly.Abstractions;

namespace Api.Features.Teams;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class TeamMapper
{
  public partial Team CreateToEntity(CreateTeamRequest request);
  public partial void UpdateEntityFromRequest(UpdateTeamRequest request, Team entity);
  public partial TeamResponseDto EntityToResponseDto(Team entity);
  public partial TeamPreviewDto EntityToPreviewDto(Team entity);
  public partial CreatedTeamResponseDto EntityToCreatedResponseDto(Team entity);
  public partial List<TeamResponseDto> EntityToResponseDtoList(List<Team> entities);
  public partial List<TeamPreviewDto> EntityToPreviewDtoList(List<Team> entities);
}
