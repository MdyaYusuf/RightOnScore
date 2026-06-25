using Api.Features.Users;

namespace Api.Features.Authentication;

// Responses
public record TokenResponseDto(
  string AccessToken,
  DateTime Expiration,
  string RefreshToken,
  UserResponseDto User);

// Requests
public sealed record LoginRequest(
  string Email,
  string Password);

public sealed record RegisterUserRequest(
  string Username,
  string Email,
  string Password);

public sealed record RefreshTokenRequest(
  string? RefreshToken);