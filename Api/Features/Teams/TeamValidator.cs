using FluentValidation;

namespace Api.Features.Teams;

public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
  public CreateTeamRequestValidator()
  {
    RuleFor(t => t.Name)
      .NotEmpty().WithMessage("Takım adı boş olamaz.")
      .NotNull().WithMessage("Takım adı zorunludur.")
      .MinimumLength(2).WithMessage("Takım adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Takım adı 100 karakterden uzun olamaz.");

    RuleFor(t => t.ShortName)
      .NotEmpty().WithMessage("Takım kısa adı boş olamaz.")
      .NotNull().WithMessage("Takım kısa adı zorunludur.")
      .MinimumLength(2).WithMessage("Takım kısa adı en az 2 karakter olmalıdır.")
      .MaximumLength(30).WithMessage("Takım kısa adı 30 karakterden uzun olamaz.");

    RuleFor(t => t.Country)
      .NotEmpty().WithMessage("Takım ülkesi boş olamaz.")
      .NotNull().WithMessage("Takım ülkesi zorunludur.")
      .MinimumLength(2).WithMessage("Takım ülkesi en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Takım ülkesi 100 karakterden uzun olamaz.");
  }
}

public class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
{
  public UpdateTeamRequestValidator()
  {
    RuleFor(t => t.Id)
      .NotEmpty().WithMessage("Geçersiz takım ID.");

    RuleFor(t => t.Name)
      .NotEmpty().WithMessage("Takım adı boş olamaz.")
      .MinimumLength(2).WithMessage("Takım adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Takım adı 100 karakterden uzun olamaz.");

    RuleFor(t => t.ShortName)
      .NotEmpty().WithMessage("Takım kısa adı boş olamaz.")
      .MinimumLength(2).WithMessage("Takım kısa adı en az 2 karakter olmalıdır.")
      .MaximumLength(30).WithMessage("Takım kısa adı 30 karakterden uzun olamaz.");

    RuleFor(t => t.Country)
      .NotEmpty().WithMessage("Takım ülkesi boş olamaz.")
      .MinimumLength(2).WithMessage("Takım ülkesi en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Takım ülkesi 100 karakterden uzun olamaz.");
  }
}

public class ChangeTeamStatusRequestValidator : AbstractValidator<ChangeTeamStatusRequest>
{
  public ChangeTeamStatusRequestValidator()
  {
    RuleFor(t => t.Id)
      .NotEmpty().WithMessage("Geçersiz takım ID.");
  }
}

public class SearchTeamRequestValidator : AbstractValidator<SearchTeamRequest>
{
  public SearchTeamRequestValidator()
  {
    RuleFor(t => t.SearchTerm)
      .NotEmpty().WithMessage("Arama metni boş olamaz.")
      .MinimumLength(2).WithMessage("Arama metni en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Arama metni 100 karakterden uzun olamaz.");
  }
}
