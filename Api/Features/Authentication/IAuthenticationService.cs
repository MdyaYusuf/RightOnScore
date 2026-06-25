using Api.Core.Responses;
using Api.Features.Users;

namespace Api.Features.Authentication;

public interface IAuthenticationService
{
  Task<ReturnModel<TokenResponseDto>> LoginAsync(
    LoginRequest request,
    CancellationToken cancellationToken);

  Task<ReturnModel<CreatedUserResponseDto>> RegisterAsync(
    RegisterUserRequest request,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<TokenResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

  Task<ReturnModel<NoData>> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}