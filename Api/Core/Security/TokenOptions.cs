namespace Api.Core.Security;

public class TokenOptions
{
  public string Audience { get; set; } = default!;
  public string Issuer { get; set; } = default!;
  public int AccessTokenExpiration { get; set; }
  public int RefreshTokenExpiration { get; set; }
  public string SecurityKey { get; set; } = default!;
}