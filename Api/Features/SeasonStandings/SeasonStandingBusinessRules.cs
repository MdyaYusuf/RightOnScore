using Api.Core.Exceptions;
using Api.Features.CompetitionSeasons;

namespace Api.Features.SeasonStandings;

public class SeasonStandingBusinessRules(
  ISeasonStandingRepository _seasonStandingRepository,
  ICompetitionSeasonRepository _competitionSeasonRepository)
{
  public async Task CompetitionSeasonMustExistAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionSeasonRepository.AnyAsync(
      cs => cs.Id == competitionSeasonId,
      cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{competitionSeasonId} numaralı yarışma sezonu bulunamadı.");
    }
  }

  public async Task<SeasonStanding> GetStandingIfExistAsync(
    Guid userId,
    Guid competitionSeasonId,
    CancellationToken cancellationToken = default)
  {
    var standing = await _seasonStandingRepository.GetByUserIdAndCompetitionSeasonIdAsync(
      userId,
      competitionSeasonId,
      cancellationToken: cancellationToken);

    if (standing == null)
    {
      throw new NotFoundException("Bu sezon için sıralama kaydınız bulunamadı.");
    }

    return standing;
  }

  public void TopCountMustBeValid(int topCount)
  {
    if (topCount < 1 || topCount > 100)
    {
      throw new BusinessException("Sıralama listesi için 1 ile 100 arasında bir değer kullanılmalıdır.");
    }
  }
}
