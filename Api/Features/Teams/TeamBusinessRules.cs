using Api.Core.Exceptions;

namespace Api.Features.Teams;

public class TeamBusinessRules(ITeamRepository _teamRepository)
{
  public async Task<Team> GetTeamIfExistAsync(
    Guid id,
    Func<IQueryable<Team>, IQueryable<Team>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var team = await _teamRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (team == null)
    {
      throw new NotFoundException($"{id} numaralı takım bulunamadı.");
    }

    return team;
  }

  public async Task NameMustBeUniqueForCountryAsync(string name, string country, CancellationToken cancellationToken = default)
  {
    var exists = await _teamRepository.AnyAsync(t => t.Name == name && t.Country == country, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu ülke için benzersiz bir takım adı kullanılmalıdır.");
    }
  }

  public async Task TeamNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, string name, string country, CancellationToken cancellationToken = default)
  {
    var exists = await _teamRepository.AnyAsync(t => t.Id != id && t.Name == name && t.Country == country, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu ülke için benzersiz bir takım adı kullanılmalıdır.");
    }
  }

  public async Task ShortNameMustBeUniqueForCountryAsync(string shortName, string country, CancellationToken cancellationToken = default)
  {
    var exists = await _teamRepository.AnyAsync(t => t.ShortName == shortName && t.Country == country, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu ülke için benzersiz bir kısa takım adı kullanılmalıdır.");
    }
  }

  public async Task TeamShortNameCannotBeDuplicatedWhenUpdatedAsync(Guid id, string shortName, string country, CancellationToken cancellationToken = default)
  {
    var exists = await _teamRepository.AnyAsync(t => t.Id != id && t.ShortName == shortName && t.Country == country, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu ülke için benzersiz bir kısa takım adı kullanılmalıdır.");
    }
  }

  public void TeamMustBeActiveForCompetitionRegistration(Team team)
  {
    if (!team.IsActive)
    {
      throw new BusinessException("Pasif takımlar yarışmalara eklenemez.");
    }
  }

  public void AdminRoleRequired(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Takım işlemleri için yetkiniz bulunmamaktadır.");
    }
  }
}
