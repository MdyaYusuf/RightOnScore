using Api.Core.Exceptions;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.Teams;

namespace Api.Features.CompetitionTeams;

public class CompetitionTeamBusinessRules(
  ICompetitionTeamRepository _competitionTeamRepository,
  ICompetitionSeasonRepository _competitionSeasonRepository,
  ICompetitionStageRepository _competitionStageRepository,
  ICompetitionGroupRepository _competitionGroupRepository,
  ITeamRepository _teamRepository)
{
  public async Task<CompetitionTeam> GetCompetitionTeamIfExistAsync(
    Guid id,
    Func<IQueryable<CompetitionTeam>, IQueryable<CompetitionTeam>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var competitionTeam = await _competitionTeamRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (competitionTeam == null)
    {
      throw new NotFoundException($"{id} numaralı yarışma takımı bulunamadı.");
    }

    return competitionTeam;
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

  public async Task<Team> TeamMustExistAsync(Guid teamId, CancellationToken cancellationToken = default)
  {
    var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken: cancellationToken);

    if (team == null)
    {
      throw new NotFoundException($"{teamId} numaralı takım bulunamadı.");
    }

    return team;
  }

  public void TeamMustBeActive(Team team)
  {
    if (!team.IsActive)
    {
      throw new BusinessException("Pasif takımlar yarışmalara eklenemez.");
    }
  }

  public async Task<CompetitionStage?> CompetitionStageMustBelongToSeasonAsync(
    Guid competitionSeasonId,
    Guid? competitionStageId,
    CancellationToken cancellationToken = default)
  {
    if (!competitionStageId.HasValue)
    {
      return null;
    }

    var competitionStage = await _competitionStageRepository.GetByIdAsync(competitionStageId.Value, cancellationToken: cancellationToken);

    if (competitionStage == null)
    {
      throw new NotFoundException($"{competitionStageId} numaralı yarışma aşaması bulunamadı.");
    }

    if (competitionStage.CompetitionSeasonId != competitionSeasonId)
    {
      throw new BusinessException("Yarışma aşaması seçilen sezona ait olmalıdır.");
    }

    return competitionStage;
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

  public async Task<CompetitionGroup?> CompetitionGroupMustBelongToStageAsync(
    CompetitionStage? competitionStage,
    Guid? competitionGroupId,
    CancellationToken cancellationToken = default)
  {
    if (!competitionGroupId.HasValue)
    {
      return null;
    }

    if (competitionStage == null)
    {
      throw new BusinessException("Grup seçildiğinde yarışma aşaması da seçilmelidir.");
    }

    var competitionGroup = await _competitionGroupRepository.GetByIdAsync(competitionGroupId.Value, cancellationToken: cancellationToken);

    if (competitionGroup == null)
    {
      throw new NotFoundException($"{competitionGroupId} numaralı yarışma grubu bulunamadı.");
    }

    if (competitionGroup.CompetitionStageId != competitionStage.Id)
    {
      throw new BusinessException("Yarışma grubu seçilen aşamaya ait olmalıdır.");
    }

    return competitionGroup;
  }

  public async Task<CompetitionGroup> CompetitionGroupMustExistAsync(Guid competitionGroupId, CancellationToken cancellationToken = default)
  {
    var competitionGroup = await _competitionGroupRepository.GetByIdAsync(competitionGroupId, cancellationToken: cancellationToken);

    if (competitionGroup == null)
    {
      throw new NotFoundException($"{competitionGroupId} numaralı yarışma grubu bulunamadı.");
    }

    return competitionGroup;
  }

  public void GroupAssignmentMustUseGroupStage(CompetitionStage? competitionStage, CompetitionGroup? competitionGroup)
  {
    if (competitionGroup != null && competitionStage?.Type != CompetitionStageType.GroupStage)
    {
      throw new BusinessException("Grup ataması yalnızca grup aşaması için yapılabilir.");
    }
  }

  public void ActiveMembershipRequiresActiveParents(
    bool isActive,
    CompetitionSeason competitionSeason,
    CompetitionStage? competitionStage,
    CompetitionGroup? competitionGroup)
  {
    if (!isActive)
    {
      return;
    }

    if (!competitionSeason.IsActive)
    {
      throw new BusinessException("Pasif sezona aktif takım kaydı yapılamaz.");
    }

    if (competitionStage != null && !competitionStage.IsActive)
    {
      throw new BusinessException("Pasif aşamaya aktif takım kaydı yapılamaz.");
    }

    if (competitionGroup != null && !competitionGroup.IsActive)
    {
      throw new BusinessException("Pasif gruba aktif takım kaydı yapılamaz.");
    }
  }

  public async Task TeamMustBeUniqueInSeasonAsync(Guid competitionSeasonId, Guid teamId, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionTeamRepository.AnyAsync(
      ct => ct.CompetitionSeasonId == competitionSeasonId && ct.TeamId == teamId,
      cancellationToken);

    if (exists)
    {
      throw new BusinessException("Takım bu sezona zaten eklenmiş.");
    }
  }

  public async Task TeamCannotBeDuplicatedWhenUpdatedAsync(Guid id, Guid competitionSeasonId, Guid teamId, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionTeamRepository.AnyAsync(
      ct => ct.Id != id && ct.CompetitionSeasonId == competitionSeasonId && ct.TeamId == teamId,
      cancellationToken);

    if (exists)
    {
      throw new BusinessException("Takım bu sezona zaten eklenmiş.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Yarışma takımı işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
