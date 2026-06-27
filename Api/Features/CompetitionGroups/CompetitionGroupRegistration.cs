namespace Api.Features.CompetitionGroups;

public static class CompetitionGroupRegistration
{
  public static IServiceCollection AddCompetitionGroupDependencies(this IServiceCollection services)
  {
    services.AddScoped<ICompetitionGroupRepository, EfCompetitionGroupRepository>();
    services.AddScoped<CompetitionGroupBusinessRules>();
    services.AddScoped<ICompetitionGroupService, CompetitionGroupService>();
    services.AddSingleton<CompetitionGroupMapper>();

    return services;
  }
}
