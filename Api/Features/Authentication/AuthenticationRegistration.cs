namespace Api.Features.Authentication;

public static class AuthenticationRegistration
{
  public static IServiceCollection AddAuthenticationDependencies(this IServiceCollection services)
  {
    services.AddScoped<AuthenticationBusinessRules>();
    services.AddScoped<IAuthenticationService, AuthenticationService>();

    return services;
  }
}