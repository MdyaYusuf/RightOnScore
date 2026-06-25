using Api.Core.Exceptions;
using Api.Core.Security;
using Api.Features.Users;

namespace Api.Features.Authentication;

public class AuthenticationBusinessRules
{
  public void UserCredentialsMustMatch(User? user, string password)
  {
    if (user == null || !HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordKey))
    {
      throw new BusinessException("E-posta adresi veya şifre hatalı.");
    }

    if (!user.IsActive)
    {
      throw new BusinessException("Hesabınız pasif durumdadır. Lütfen yönetici ile iletişime geçin.");
    }
  }

  public void RefreshTokenMustBeValid(User? user)
  {
    if (user == null || user.RefreshTokenExpiration < DateTime.UtcNow)
    {
      throw new AuthorizationException("Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.");
    }
  }

  public void RefreshTokenUserMustExist(User? user)
  {
    if (user == null)
    {
      throw new NotFoundException("Oturum bilgisi bulunamadı.");
    }
  }
}