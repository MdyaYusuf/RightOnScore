using Api.Core.Exceptions;
using Api.Features.CompetitionSeasons;
using Api.Features.Matches;
using Api.Features.Users;

namespace Api.Features.MatchPredictions;

public class MatchPredictionBusinessRules(
  IMatchPredictionRepository _matchPredictionRepository,
  IMatchRepository _matchRepository,
  IUserRepository _userRepository,
  ICompetitionSeasonRepository _competitionSeasonRepository)
{
  public async Task<MatchPrediction> GetMatchPredictionIfExistAsync(
    Guid id,
    Func<IQueryable<MatchPrediction>, IQueryable<MatchPrediction>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var prediction = await _matchPredictionRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (prediction == null)
    {
      throw new NotFoundException($"{id} numaralı skor tahmini bulunamadı.");
    }

    return prediction;
  }

  public async Task<Match> MatchMustExistAsync(
    Guid matchId,
    Func<IQueryable<Match>, IQueryable<Match>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var match = await _matchRepository.GetByIdAsync(matchId, include, enableTracking, cancellationToken);

    if (match == null)
    {
      throw new NotFoundException($"{matchId} numaralı maç bulunamadı.");
    }

    return match;
  }

  public async Task<User> UserMustExistAndBeActiveAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetByIdAsync(userId, cancellationToken: cancellationToken);

    if (user == null)
    {
      throw new NotFoundException($"{userId} numaralı kullanıcı bulunamadı.");
    }

    if (!user.IsActive)
    {
      throw new BusinessException("Pasif kullanıcılar skor tahmini yapamaz.");
    }

    return user;
  }

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

  public void UserMustOwnPredictionOrBeAdmin(MatchPrediction prediction, Guid currentUserId, string userRole)
  {
    if (prediction.UserId != currentUserId && userRole != "Admin")
    {
      throw new ForbiddenException("Bu skor tahmini üzerinde işlem yapma yetkiniz bulunmamaktadır.");
    }
  }

  public void UserMustBeOwnerOrAdmin(Guid targetUserId, Guid currentUserId, string userRole)
  {
    if (targetUserId != currentUserId && userRole != "Admin")
    {
      throw new ForbiddenException("Bu işlem için yetkiniz bulunmamaktadır.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Bu işlem için yönetici yetkisi gerekmektedir.");
    }
  }

  public void PredictedScoresMustBeValid(int predictedHomeScore, int predictedAwayScore)
  {
    if (predictedHomeScore < 0 || predictedAwayScore < 0)
    {
      throw new BusinessException("Tahmin edilen skorlar negatif olamaz.");
    }
  }

  public void KnockoutPredictionMustBeValid(
    Match match,
    int predictedHomeScore,
    int predictedAwayScore,
    Guid? predictedAdvancingTeamId)
  {
    if (!MatchPredictionScoringRules.IsKnockoutStage(match.CompetitionStage))
    {
      if (predictedAdvancingTeamId.HasValue)
      {
        throw new BusinessException("Tur atlayan takım tahmini yalnızca eleme maçlarında yapılabilir.");
      }

      return;
    }

    bool isPredictedDraw = predictedHomeScore == predictedAwayScore;

    if (isPredictedDraw)
    {
      if (!predictedAdvancingTeamId.HasValue)
      {
        throw new BusinessException("Beraberlik tahmini için tur atlayan takım seçilmelidir.");
      }

      if (predictedAdvancingTeamId.Value != match.HomeTeamId && predictedAdvancingTeamId.Value != match.AwayTeamId)
      {
        throw new BusinessException("Tur atlayan takım ev sahibi veya deplasman takımlarından biri olmalıdır.");
      }

      return;
    }

    if (predictedAdvancingTeamId.HasValue)
    {
      throw new BusinessException("Galibiyet tahminlerinde tur atlayan takım ayrıca seçilmemelidir.");
    }
  }

  public void MatchMustBeOpenForPredictions(Match match)
  {
    if (match.Status != MatchStatus.Scheduled)
    {
      throw new BusinessException("Yalnızca planlanmış maçlar için skor tahmini yapılabilir.");
    }

    if (match.KickoffTime <= DateTime.Now)
    {
      throw new BusinessException("Maç başladıktan sonra skor tahmini yapılamaz veya güncellenemez.");
    }
  }

  public void PredictionCanOnlyBeModifiedBeforeKickoff(Match match)
  {
    if (match.KickoffTime <= DateTime.Now)
    {
      throw new BusinessException("Maç başladıktan sonra skor tahmini güncellenemez veya silinemez.");
    }
  }

  public async Task UserMustNotHavePredictionForMatchAsync(
    Guid userId,
    Guid matchId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _matchPredictionRepository.AnyAsync(
      p => p.UserId == userId && p.MatchId == matchId,
      cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu maç için zaten bir skor tahmininiz bulunmaktadır.");
    }
  }
}
