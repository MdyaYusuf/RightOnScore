using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Api.Core.Middlewares;
using Api.Core.Security;
using Api.Data;
using Api.Features.Authentication;
using Api.Features.CompetitionGroups;
using Api.Features.CompetitionSeasons;
using Api.Features.CompetitionStages;
using Api.Features.CompetitionTeams;
using Api.Features.Competitions;
using Api.Features.Matches;
using Api.Features.MatchPredictions;
using Api.Features.Roles;
using Api.Features.SeasonStandings;
using Api.Features.Teams;
using Api.Features.Users;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddDataDependencies(builder.Configuration);
builder.Services.AddRoleDependencies();
builder.Services.AddUserDependencies();
builder.Services.AddAuthenticationDependencies();
builder.Services.AddCompetitionDependencies();
builder.Services.AddCompetitionSeasonDependencies();
builder.Services.AddCompetitionStageDependencies();
builder.Services.AddCompetitionGroupDependencies();
builder.Services.AddCompetitionTeamDependencies();
builder.Services.AddSeasonStandingDependencies();
builder.Services.AddMatchPredictionDependencies();
builder.Services.AddMatchDependencies();
builder.Services.AddTeamDependencies();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>() ?? throw new InvalidOperationException("TokenOptions bölümü yapılandırma dosyasında appsettings bulunamadı.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidIssuer = tokenOptions.Issuer,
      ValidAudience = tokenOptions.Audience,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
      ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
      OnMessageReceived = context =>
      {
        if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.TryGetValue("accessToken", out var token))
        {
          context.Token = token;
        }
        return Task.CompletedTask;
      }
    };
  });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();