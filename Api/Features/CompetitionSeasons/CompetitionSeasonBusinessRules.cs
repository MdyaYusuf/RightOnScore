using Api.Core.Exceptions;
using Api.Features.Competitions;

namespace Api.Features.CompetitionSeasons;

public class CompetitionSeasonBusinessRules(
  ICompetitionSeasonRepository _competitionSeasonRepository,
  ICompetitionRepository _competitionRepository)
{
  public async Task<CompetitionSeason> GetCompetitionSeasonIfExistAsync(
    Guid id,
    Func<IQueryable<CompetitionSeason>, IQueryable<CompetitionSeason>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var competitionSeason = await _competitionSeasonRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (competitionSeason == null)
    {
      throw new NotFoundException($"{id} numaralı yarışma sezonu bulunamadı.");
    }

    return competitionSeason;
  }

  public async Task CompetitionMustExistAsync(Guid competitionId, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionRepository.AnyAsync(c => c.Id == competitionId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{competitionId} numaralı yarışma bulunamadı.");
    }
  }

  public async Task CompetitionMustBeActiveAsync(Guid competitionId, CancellationToken cancellationToken = default)
  {
    var competition = await _competitionRepository.GetByIdAsync(competitionId, cancellationToken: cancellationToken);

    if (competition == null)
    {
      throw new NotFoundException($"{competitionId} numaralı yarışma bulunamadı.");
    }

    if (!competition.IsActive)
    {
      throw new BusinessException("Pasif yarışmalara sezon eklenemez veya aktif sezon atanamaz.");
    }
  }

  public async Task NameMustBeUniqueForCompetitionAsync(Guid competitionId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionSeasonRepository.AnyAsync(cs => cs.CompetitionId == competitionId && cs.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu yarışma için benzersiz bir sezon adı kullanılmalıdır.");
    }
  }

  public async Task SeasonNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionSeasonRepository.AnyAsync(cs => cs.Id != id && cs.CompetitionId == competitionId && cs.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu yarışma için benzersiz bir sezon adı kullanılmalıdır.");
    }
  }

  public void SeasonDateRangeMustBeValid(DateTime startDate, DateTime endDate)
  {
    if (startDate >= endDate)
    {
      throw new BusinessException("Sezon başlangıç tarihi bitiş tarihinden önce olmalıdır.");
    }
  }

  public async Task OnlyOneActiveSeasonPerCompetitionAsync(Guid competitionId, Guid? ignoredSeasonId = null, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionSeasonRepository.AnyAsync(
      cs => cs.CompetitionId == competitionId && cs.IsActive && (!ignoredSeasonId.HasValue || cs.Id != ignoredSeasonId.Value),
      cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bir yarışmanın aynı anda yalnızca bir aktif sezonu olabilir.");
    }
  }

  public void ActiveSeasonMustHaveActiveStatus(bool isActive, CompetitionSeasonStatus status)
  {
    if (isActive && status != CompetitionSeasonStatus.Active)
    {
      throw new BusinessException("Aktif olarak işaretlenen sezonun durumu Active olmalıdır.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Yarışma sezonu işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
