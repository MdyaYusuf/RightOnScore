namespace Api.Features.Competitions;

public static class CompetitionRegistration
{
  public static IServiceCollection AddCompetitionDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICompetitionRepository, EfCompetitionRepository>();
    services.AddScoped<CompetitionBusinessRules>();
    services.AddScoped<ICompetitionService, CompetitionService>();
    services.AddSingleton<CompetitionMapper>();

    return services;
  }
}
