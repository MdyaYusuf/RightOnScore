namespace Api.Features.MatchPredictions;

public static class MatchPredictionRegistration
{
  public static IServiceCollection AddMatchPredictionDependencies(this IServiceCollection services)
  {
    services.AddScoped<IMatchPredictionRepository, EfMatchPredictionRepository>();
    services.AddScoped<MatchPredictionBusinessRules>();
    services.AddScoped<MatchPredictionScoringBusinessRules>();
    services.AddScoped<IMatchPredictionScoringService, MatchPredictionScoringService>();
    services.AddScoped<IMatchPredictionService, MatchPredictionService>();
    services.AddSingleton<MatchPredictionMapper>();

    return services;
  }
}
