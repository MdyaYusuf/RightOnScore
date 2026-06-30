namespace Api.Features.CompetitionTeams;

public static class CompetitionTeamRegistration
{
  public static IServiceCollection AddCompetitionTeamDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICompetitionTeamRepository, EfCompetitionTeamRepository>();
    services.AddScoped<CompetitionTeamBusinessRules>();
    services.AddScoped<ICompetitionTeamService, CompetitionTeamService>();
    services.AddSingleton<CompetitionTeamMapper>();

    return services;
  }
}
