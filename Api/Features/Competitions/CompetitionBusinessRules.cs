using Api.Core.Exceptions;

namespace Api.Features.Competitions;

public class CompetitionBusinessRules(ICompetitionRepository _competitionRepository)
{
  public async Task<Competition> GetCompetitionIfExistAsync(
    Guid id,
    Func<IQueryable<Competition>, IQueryable<Competition>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var competition = await _competitionRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (competition == null)
    {
      throw new NotFoundException($"{id} numaralı yarışma bulunamadı.");
    }

    return competition;
  }

  public async Task NameMustBeUniqueAsync(string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionRepository.AnyAsync(c => c.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Yarışma için benzersiz bir isim kullanılmalıdır.");
    }
  }

  public async Task CompetitionNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, string name, CancellationToken cancellationToken = default)
  {
    var exists = await _competitionRepository.AnyAsync(c => c.Id != id && c.Name == name, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Yarışma için benzersiz bir isim kullanılmalıdır.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Yarışma işlemleri için yetkiniz bulunmamaktadır.");
    }
  }

  public void CompetitionMustBeActiveForPrediction(Competition competition)
  {
    if (!competition.IsActive)
    {
      throw new BusinessException("Pasif yarışmalar için tahmin yapılamaz.");
    }
  }
}
