using Api.Core.Controllers;
using Api.Core.Responses;
using Api.Core.Security;
using Api.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Features.Authentication;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
  IAuthenticationService _authService,
  IOptions<TokenOptions> _tokenOptions,
  IWebHostEnvironment _environment) : CustomBaseController
{
  private readonly TokenOptions _options = _tokenOptions.Value;

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var result = await _authService.LoginAsync(request, cancellationToken);

    if (!result.Success || result.Data == null)
    {
      return CreateActionResult(result);
    }

    SetTokensAsCookies(result.Data);

    return CreateClientAwareResult(result);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
  {
    var result = await _authService.RegisterAsync(request, cancellationToken);

    return CreateActionResult(result);
  }

  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
  {
    var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

    if (string.IsNullOrEmpty(refreshToken))
    {
      return CreateActionResult(new ReturnModel<TokenResponseDto>
      {
        Success = false,
        StatusCode = 401,
        Message = "Oturum bulunamadı veya süresi dolmuş."
      });
    }

    var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

    if (!result.Success || result.Data == null)
    {
      return CreateActionResult(result);
    }

    SetTokensAsCookies(result.Data);

    return CreateClientAwareResult(result);
  }

  [HttpPost("revoke-refresh-token")]
  public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
  {
    var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

    if (!string.IsNullOrEmpty(refreshToken))
    {
      await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
    }

    Response.Cookies.Delete("accessToken");
    Response.Cookies.Delete("refreshToken");

    return CreateActionResult(new ReturnModel<NoData>
    {
      Success = true,
      StatusCode = 200,
      Message = "Başarıyla çıkış yapıldı."
    });
  }

  private void SetTokensAsCookies(TokenResponseDto tokens)
  {
    // Secure cookies are rejected on http://localhost; keep Secure outside Development.
    bool secureCookies = !_environment.IsDevelopment();

    var accessOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = secureCookies,
      SameSite = SameSiteMode.Strict,
      Expires = tokens.Expiration
    };
    Response.Cookies.Append("accessToken", tokens.AccessToken, accessOptions);

    var refreshOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = secureCookies,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddDays(_options.RefreshTokenExpiration)
    };
    Response.Cookies.Append("refreshToken", tokens.RefreshToken, refreshOptions);
  }

  private IActionResult CreateClientAwareResult(ReturnModel<TokenResponseDto> result)
  {
    bool isMobile = Request.Headers["X-Client-Platform"] == "Mobile";

    if (isMobile)
    {
      return CreateActionResult(result);
    }

    var safeWebResponse = new ReturnModel<UserResponseDto>
    {
      Success = result.Success,
      Message = result.Message,
      StatusCode = result.StatusCode,
      Data = result.Data!.User
    };

    return CreateActionResult(safeWebResponse);
  }
}