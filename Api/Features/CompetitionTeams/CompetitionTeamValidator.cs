using FluentValidation;

namespace Api.Features.CompetitionTeams;

public class CreateCompetitionTeamRequestValidator : AbstractValidator<CreateCompetitionTeamRequest>
{
  public CreateCompetitionTeamRequestValidator()
  {
    RuleFor(ct => ct.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(ct => ct.TeamId)
      .NotEmpty().WithMessage("Geçersiz takım ID.");

    RuleFor(ct => ct.Seed)
      .GreaterThan(0).WithMessage("Takım seri numarası 0'dan büyük olmalıdır.")
      .When(ct => ct.Seed.HasValue);
  }
}

public class UpdateCompetitionTeamRequestValidator : AbstractValidator<UpdateCompetitionTeamRequest>
{
  public UpdateCompetitionTeamRequestValidator()
  {
    RuleFor(ct => ct.Id)
      .NotEmpty().WithMessage("Geçersiz yarışma takımı ID.");

    RuleFor(ct => ct.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(ct => ct.TeamId)
      .NotEmpty().WithMessage("Geçersiz takım ID.");

    RuleFor(ct => ct.Seed)
      .GreaterThan(0).WithMessage("Takım seri numarası 0'dan büyük olmalıdır.")
      .When(ct => ct.Seed.HasValue);
  }
}

public class ChangeCompetitionTeamStatusRequestValidator : AbstractValidator<ChangeCompetitionTeamStatusRequest>
{
  public ChangeCompetitionTeamStatusRequestValidator()
  {
    RuleFor(ct => ct.Id)
      .NotEmpty().WithMessage("Geçersiz yarışma takımı ID.");
  }
}
