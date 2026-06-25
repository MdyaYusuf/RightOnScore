namespace Api.Features.Roles;

public static class RoleRegistration
{
  public static IServiceCollection AddRoleDependencies(this IServiceCollection services)
  {
    services.AddScoped<IRoleRepository, EfRoleRepository>();
    services.AddScoped<RoleBusinessRules>();
    services.AddScoped<IRoleService, RoleService>();
    services.AddSingleton<RoleMapper>();

    return services;
  }
}