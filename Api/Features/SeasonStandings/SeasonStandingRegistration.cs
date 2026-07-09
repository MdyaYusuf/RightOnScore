namespace Api.Features.SeasonStandings;

public static class SeasonStandingRegistration
{
  public static IServiceCollection AddSeasonStandingDependencies(this IServiceCollection services)
  {
    services.AddScoped<ISeasonStandingRepository, EfSeasonStandingRepository>();
    services.AddScoped<SeasonStandingBusinessRules>();
    services.AddScoped<ISeasonStandingService, SeasonStandingService>();
    services.AddSingleton<SeasonStandingMapper>();

    return services;
  }
}
