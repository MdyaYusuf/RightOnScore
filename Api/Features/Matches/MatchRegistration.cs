namespace Api.Features.Matches;

public static class MatchRegistration
{
  public static IServiceCollection AddMatchDependencies(this IServiceCollection services)
  {
    services.AddScoped<IMatchRepository, EfMatchRepository>();
    services.AddScoped<MatchBusinessRules>();
    services.AddScoped<IMatchService, MatchService>();
    services.AddSingleton<MatchMapper>();

    return services;
  }
}
