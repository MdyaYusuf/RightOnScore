namespace Api.Features.Teams;

public static class TeamRegistration
{
  public static IServiceCollection AddTeamDependencies(this IServiceCollection services)
  {
    services.AddScoped<ITeamRepository, EfTeamRepository>();
    services.AddScoped<TeamBusinessRules>();
    services.AddScoped<ITeamService, TeamService>();
    services.AddSingleton<TeamMapper>();

    return services;
  }
}
