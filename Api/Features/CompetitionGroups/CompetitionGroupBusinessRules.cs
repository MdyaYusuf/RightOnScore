using Api.Core.Exceptions;
using Api.Features.CompetitionStages;

namespace Api.Features.CompetitionGroups;

public class CompetitionGroupBusinessRules(
  ICompetitionGroupRepository _competitionGroupRepository,
  ICompetitionStageRepository _competitionStageRepository)
{
  public async Task<CompetitionGroup> GetCompetitionGroupIfExistAsync(
    Guid id,
    Func<IQueryable<CompetitionGroup>, IQueryable<CompetitionGroup>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var competitionGroup = await _competitionGroupRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (competitionGroup == null)
    {
      throw new NotFoundException($"{id} numaralı yarışma grubu bulunamadı.");
    }

    return competitionGroup;
  }

  public async Task<CompetitionStage> CompetitionStageMustExistAsync(Guid competitionStageId, CancellationToken cancellationToken = default)
  {
    var competitionStage = await _competitionStageRepository.GetByIdAsync(competitionStageId, cancellationToken: cancellationToken);

    if (competitionStage == null)
    {
      throw new NotFoundException($"{competitionStageId} numaralı yarışma aşaması bulunamadı.");
    }

    return competitionStage;
  }

  public void CompetitionStageMustBeGroupStage(CompetitionStage competitionStage)
  {
    if (competitionStage.Type != CompetitionStageType.GroupStage)
    {
      throw new BusinessException("Gruplar yalnızca grup aşaması tipindeki yarışma aşamalarına eklenebilir.");
    }
  }

  public void ActiveGroupRequiresActiveStage(CompetitionStage competitionStage, bool isActive)
  {
    if (isActive && !competitionStage.IsActive)
    {
      throw new BusinessException("Pasif aşamalara aktif grup eklenemez veya atanamaz.");
    }
  }

  public async Task NameMustBeUniqueForStageAsync(Guid competitionStageId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionGroupRepository.AnyAsync(cg => cg.CompetitionStageId == competitionStageId && cg.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu aşama için benzersiz bir grup adı kullanılmalıdır.");
    }
  }

  public async Task GroupNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionStageId, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionGroupRepository.AnyAsync(cg => cg.Id != id && cg.CompetitionStageId == competitionStageId && cg.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu aşama için benzersiz bir grup adı kullanılmalıdır.");
    }
  }

  public async Task DisplayOrderMustBeUniqueForStageAsync(Guid competitionStageId, int displayOrder, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionGroupRepository.AnyAsync(cg => cg.CompetitionStageId == competitionStageId && cg.DisplayOrder == displayOrder, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu aşama için benzersiz bir grup sırası kullanılmalıdır.");
    }
  }

  public async Task DisplayOrderCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionStageId, int displayOrder, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionGroupRepository.AnyAsync(cg => cg.Id != id && cg.CompetitionStageId == competitionStageId && cg.DisplayOrder == displayOrder, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu aşama için benzersiz bir grup sırası kullanılmalıdır.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Yarışma grubu işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
