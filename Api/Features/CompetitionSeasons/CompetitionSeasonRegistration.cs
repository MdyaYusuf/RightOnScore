namespace Api.Features.CompetitionSeasons;

public static class CompetitionSeasonRegistration
{
  public static IServiceCollection AddCompetitionSeasonDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICompetitionSeasonRepository, EfCompetitionSeasonRepository>();
    services.AddScoped<CompetitionSeasonBusinessRules>();
    services.AddScoped<ICompetitionSeasonService, CompetitionSeasonService>();
    services.AddSingleton<CompetitionSeasonMapper>();

    return services;
  }
}
