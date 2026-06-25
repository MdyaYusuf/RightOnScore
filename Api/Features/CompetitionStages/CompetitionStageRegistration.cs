namespace Api.Features.CompetitionStages;

public static class CompetitionStageRegistration
{
  public static IServiceCollection AddCompetitionStageDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICompetitionStageRepository, EfCompetitionStageRepository>();
    services.AddScoped<CompetitionStageBusinessRules>();
    services.AddScoped<ICompetitionStageService, CompetitionStageService>();
    services.AddSingleton<CompetitionStageMapper>();

    return services;
  }
}
