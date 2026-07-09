using Api.Features.Users;
using Riok.Mapperly.Abstractions;

namespace Api.Features.SeasonStandings;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class SeasonStandingMapper
{
  public partial SeasonStandingResponseDto EntityToResponseDto(SeasonStanding entity);
  public partial SeasonStandingPreviewDto EntityToPreviewDto(SeasonStanding entity);
  [MapProperty("Role.Name", "RoleName")]
  public partial UserPreviewDto UserToPreviewDto(User entity);
  public partial List<SeasonStandingPreviewDto> EntityToPreviewDtoList(List<SeasonStanding> entities);
}
