using Api.Core.Exceptions;
using Api.Features.CompetitionSeasons;

namespace Api.Features.CompetitionStages;

public class CompetitionStageBusinessRules(
  ICompetitionStageRepository _competitionStageRepository,
  ICompetitionSeasonRepository _competitionSeasonRepository)
{
  public async Task<CompetitionStage> GetCompetitionStageIfExistAsync(
    Guid id,
    Func<IQueryable<CompetitionStage>, IQueryable<CompetitionStage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var competitionStage = await _competitionStageRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (competitionStage == null)
    {
      throw new NotFoundException($"{id} numaralı yarışma aşaması bulunamadı.");
    }

    return competitionStage;
  }

  public async Task<CompetitionSeason> CompetitionSeasonMustExistAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var competitionSeason = await _competitionSeasonRepository.GetByIdAsync(competitionSeasonId, cancellationToken: cancellationToken);

    if (competitionSeason == null)
    {
      throw new NotFoundException($"{competitionSeasonId} numaralı yarışma sezonu bulunamadı.");
    }

    return competitionSeason;
  }

  public async Task CompetitionSeasonMustBeActiveAsync(Guid competitionSeasonId, CancellationToken cancellationToken = default)
  {
    var competitionSeason = await CompetitionSeasonMustExistAsync(competitionSeasonId, cancellationToken);

    if (!competitionSeason.IsActive)
    {
      throw new BusinessException("Pasif sezonlara aktif aşama eklenemez veya atanamaz.");
    }
  }

  public async Task NameMustBeUniqueForSeasonAsync(Guid competitionSeasonId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionStageRepository.AnyAsync(cs => cs.CompetitionSeasonId == competitionSeasonId && cs.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu sezon için benzersiz bir aşama adı kullanılmalıdır.");
    }
  }

  public async Task StageNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionSeasonId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionStageRepository.AnyAsync(cs => cs.Id != id && cs.CompetitionSeasonId == competitionSeasonId && cs.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu sezon için benzersiz bir aşama adı kullanılmalıdır.");
    }
  }

  public async Task DisplayOrderMustBeUniqueForSeasonAsync(Guid competitionSeasonId, int displayOrder, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionStageRepository.AnyAsync(cs => cs.CompetitionSeasonId == competitionSeasonId && cs.DisplayOrder == displayOrder, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu sezon için benzersiz bir aşama sırası kullanılmalıdır.");
    }
  }

  public async Task DisplayOrderCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionSeasonId, int displayOrder, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionStageRepository.AnyAsync(cs => cs.Id != id && cs.CompetitionSeasonId == competitionSeasonId && cs.DisplayOrder == displayOrder, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu sezon için benzersiz bir aşama sırası kullanılmalıdır.");
    }
  }

  public void StageDateRangeMustBeValid(DateTime startDate, DateTime endDate)
  {
    if (startDate >= endDate)
    {
      throw new BusinessException("Aşama başlangıç tarihi bitiş tarihinden önce olmalıdır.");
    }
  }

  public void StageDateRangeMustBeInsideSeason(CompetitionSeason competitionSeason, DateTime startDate, DateTime endDate)
  {
    if (startDate < competitionSeason.StartDate || endDate > competitionSeason.EndDate)
    {
      throw new BusinessException("Aşama tarihleri sezon tarih aralığı içinde olmalıdır.");
    }
  }

  public void ActiveStageMustHaveActiveStatus(bool isActive, CompetitionStageStatus status)
  {
    if (isActive && status != CompetitionStageStatus.Active)
    {
      throw new BusinessException("Aktif olarak işaretlenen aşamanın durumu Active olmalıdır.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Yarışma aşaması işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
