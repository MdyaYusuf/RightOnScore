using Api.Core.Exceptions;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.CompetitionTeams;
using Api.Features.Teams;

namespace Api.Features.Matches;

public class MatchBusinessRules(
  IMatchRepository _matchRepository,
  ICompetitionSeasonRepository _competitionSeasonRepository,
  ICompetitionStageRepository _competitionStageRepository,
  ICompetitionGroupRepository _competitionGroupRepository,
  ICompetitionTeamRepository _competitionTeamRepository,
  ITeamRepository _teamRepository)
{
  public async Task<Match> GetMatchIfExistAsync(
    Guid id,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var match = await _matchRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (match == null)
    {
      throw new NotFoundException($"{id} numaralı maç bulunamadı.");
    }

    return match;
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

  public void HomeAndAwayTeamsMustBeDifferent(Guid homeTeamId, Guid awayTeamId)
  {
    if (homeTeamId == awayTeamId)
    {
      throw new BusinessException("Ev sahibi ve deplasman takımları farklı olmalıdır.");
    }
  }

  public async Task TeamsMustBeRegisteredInSeasonAsync(
    Guid competitionSeasonId,
    Guid homeTeamId,
    Guid awayTeamId,
    CancellationToken cancellationToken = default)
  {
    var homeRegistered = await _competitionTeamRepository.AnyAsync(
      ct => ct.CompetitionSeasonId == competitionSeasonId && ct.TeamId == homeTeamId && ct.IsActive,
      cancellationToken);

    if (!homeRegistered)
    {
      throw new BusinessException("Ev sahibi takım bu sezona kayıtlı ve aktif olmalıdır.");
    }

    var awayRegistered = await _competitionTeamRepository.AnyAsync(
      ct => ct.CompetitionSeasonId == competitionSeasonId && ct.TeamId == awayTeamId && ct.IsActive,
      cancellationToken);

    if (!awayRegistered)
    {
      throw new BusinessException("Deplasman takımı bu sezona kayıtlı ve aktif olmalıdır.");
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

  public void KickoffMustBeWithinSeason(CompetitionSeason competitionSeason, DateTime kickoffTime)
  {
    if (kickoffTime < competitionSeason.StartDate || kickoffTime > competitionSeason.EndDate)
    {
      throw new BusinessException("Maç başlama saati sezon tarih aralığı içinde olmalıdır.");
    }
  }

  public void KickoffMustBeWithinStage(CompetitionStage? competitionStage, DateTime kickoffTime)
  {
    if (competitionStage == null)
    {
      return;
    }

    if (kickoffTime < competitionStage.StartDate || kickoffTime > competitionStage.EndDate)
    {
      throw new BusinessException("Maç başlama saati aşama tarih aralığı içinde olmalıdır.");
    }
  }

  public void FixtureCanOnlyBeModifiedWhenScheduledOrPostponed(Match match)
  {
    if (match.Status is not (MatchStatus.Scheduled or MatchStatus.Postponed))
    {
      throw new BusinessException("Yalnızca planlanmış veya ertelenmiş maçların fikstür bilgileri güncellenebilir.");
    }
  }

  public void ResultCanOnlyBeRecordedForLiveOrScheduledMatch(Match match)
  {
    if (match.Status is not (MatchStatus.Scheduled or MatchStatus.Live))
    {
      throw new BusinessException("Sonuç yalnızca planlanmış veya devam eden maçlar için kaydedilebilir.");
    }
  }

  public void ScoresMustBeValid(int homeScore, int awayScore)
  {
    if (homeScore < 0 || awayScore < 0)
    {
      throw new BusinessException("Maç skorları negatif olamaz.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Maç işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
