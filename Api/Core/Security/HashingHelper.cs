using System.Security.Cryptography;
using System.Text;

namespace Api.Core.Security;

public static class HashingHelper
{
  public static void CreatePasswordHash(string password, out string passwordHash, out string passwordKey)
  {
    using var hmac = new HMACSHA512();
    passwordKey = Convert.ToBase64String(hmac.Key);
    passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
  }

  public static bool VerifyPasswordHash(string password, string passwordHash, string passwordKey)
  {
    using var hmac = new HMACSHA512(Convert.FromBase64String(passwordKey));
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

    return Convert.ToBase64String(computedHash) == passwordHash;
  }

  public static string HashRefreshToken(string token)
  {
    using var sha256 = SHA256.Create();
    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

    return Convert.ToBase64String(hashBytes);
  }
}