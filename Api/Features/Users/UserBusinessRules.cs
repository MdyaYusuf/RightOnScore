using Api.Core.Exceptions;
using Api.Core.Security;
using Api.Features.Roles;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users;

public class UserBusinessRules(IUserRepository _userRepository, IRoleRepository _roleRepository)
{
  public async Task<User> GetUserIfExistAsync(
    Guid id,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (user == null)
    {
      throw new NotFoundException($"{id} numaralı kullanıcı bulunamadı.");
    }

    return user;
  }

  public async Task UserIdMustExist(Guid userId, CancellationToken cancellationToken)
  {
    bool exists = await _userRepository.AnyAsync(u => u.Id == userId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{userId} numaralı kullanıcı bulunamadı.");
    }
  }

  public async Task EmailMustBeUniqueAsync(string email, Guid? id = null, CancellationToken cancellationToken = default)
  {
    var exists = await _userRepository.AnyAsync(u => u.Email == email && (id == null || u.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu eposta adresi zaten kullanımda.");
    }
  }

  public async Task UsernameMustBeUniqueAsync(string username, Guid? id = null, CancellationToken cancellationToken = default)
  {
    var exists = await _userRepository.AnyAsync(u => u.Username == username && (id == null || u.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu kullanıcı adı zaten alınmış.");
    }
  }

  public void UserMustBeOwnerOrAdmin(Guid requestTargetId, Guid currentUserId, string userRole)
  {
    if (requestTargetId != currentUserId && userRole != "Admin")
    {
      throw new ForbiddenException("Bu işlem için yetkiniz bulunmamaktadır.");
    }
  }

  public void PasswordMustMatch(string password, string storedHash, string storedKey)
  {
    if (!HashingHelper.VerifyPasswordHash(password, storedHash, storedKey))
    {
      throw new BusinessException("Mevcut şifreniz hatalı.");
    }
  }

  public async Task RoleMustExistAsync(int roleId, CancellationToken cancellationToken)
  {
    var exists = await _roleRepository.AnyAsync(r => r.Id == roleId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException("Belirtilen rol sistemde kayıtlı değil.");
    }
  }

  public void UsernameCannotBeRestrictedWord(string username)
  {
    var restrictedWords = new[] { "admin", "administrator", "system", "root", "support", "moderator", "aksiyonfutbol" };

    if (restrictedWords.Contains(username.ToLowerInvariant()))
    {
      throw new BusinessException("Seçtiğiniz kullanıcı adı sistem tarafından rezerve edilmiştir. Lütfen farklı bir kullanıcı adı belirleyiniz.");
    }
  }

  public async Task CannotDeleteLastAdminAsync(User user, CancellationToken cancellationToken = default)
  {
    var isAdmin = await _roleRepository.AnyAsync(r => r.Id == user.RoleId && r.Name == "Admin", cancellationToken);

    if (isAdmin)
    {
      var adminCount = await _userRepository.Query(enableTracking: false)
        .CountAsync(u => u.RoleId == user.RoleId, cancellationToken);

      if (adminCount <= 1)
      {
        throw new BusinessException("Sistemdeki son yönetici (Admin) hesabı silinemez.");
      }
    }
  }
}