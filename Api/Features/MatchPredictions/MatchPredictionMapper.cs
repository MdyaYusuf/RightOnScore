using Api.Features.Matches;
using Api.Features.Users;
using Riok.Mapperly.Abstractions;

namespace Api.Features.MatchPredictions;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class MatchPredictionMapper
{
  public partial MatchPrediction CreateToEntity(CreateMatchPredictionRequest request);
  public partial void UpdateEntityFromRequest(UpdateMatchPredictionRequest request, MatchPrediction entity);
  public partial MatchPredictionResponseDto EntityToResponseDto(MatchPrediction entity);
  public partial MatchPredictionPreviewDto EntityToPreviewDto(MatchPrediction entity);
  public partial MatchPredictionRevealItemDto EntityToRevealItemDto(MatchPrediction entity);
  public partial CreatedMatchPredictionResponseDto EntityToCreatedResponseDto(MatchPrediction entity);
  [MapProperty("Role.Name", "RoleName")]
  public partial UserPreviewDto UserToPreviewDto(User entity);
  public partial MatchPreviewDto MatchToPreviewDto(Match entity);
  public partial List<MatchPredictionResponseDto> EntityToResponseDtoList(List<MatchPrediction> entities);
  public partial List<MatchPredictionPreviewDto> EntityToPreviewDtoList(List<MatchPrediction> entities);
  public partial List<MatchPredictionRevealItemDto> EntityToRevealItemDtoList(List<MatchPrediction> entities);
}
