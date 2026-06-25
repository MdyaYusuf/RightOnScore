using FluentValidation;

namespace Api.Features.CompetitionSeasons;

public class CreateCompetitionSeasonRequestValidator : AbstractValidator<CreateCompetitionSeasonRequest>
{
  public CreateCompetitionSeasonRequestValidator()
  {
    RuleFor(cs => cs.CompetitionId)
      .NotEmpty().WithMessage("Geçersiz yarışma ID.");

    RuleFor(cs => cs.Name)
      .NotEmpty().WithMessage("Sezon adı boş olamaz.")
      .NotNull().WithMessage("Sezon adı zorunludur.")
      .MinimumLength(2).WithMessage("Sezon adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Sezon adı 100 karakterden uzun olamaz.");

    RuleFor(cs => cs.StartDate)
      .NotEmpty().WithMessage("Sezon başlangıç tarihi zorunludur.");

    RuleFor(cs => cs.EndDate)
      .NotEmpty().WithMessage("Sezon bitiş tarihi zorunludur.")
      .GreaterThan(cs => cs.StartDate).WithMessage("Sezon bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz sezon durumu.");
  }
}

public class UpdateCompetitionSeasonRequestValidator : AbstractValidator<UpdateCompetitionSeasonRequest>
{
  public UpdateCompetitionSeasonRequestValidator()
  {
    RuleFor(cs => cs.Id)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(cs => cs.CompetitionId)
      .NotEmpty().WithMessage("Geçersiz yarışma ID.");

    RuleFor(cs => cs.Name)
      .NotEmpty().WithMessage("Sezon adı boş olamaz.")
      .MinimumLength(2).WithMessage("Sezon adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Sezon adı 100 karakterden uzun olamaz.");

    RuleFor(cs => cs.StartDate)
      .NotEmpty().WithMessage("Sezon başlangıç tarihi zorunludur.");

    RuleFor(cs => cs.EndDate)
      .NotEmpty().WithMessage("Sezon bitiş tarihi zorunludur.")
      .GreaterThan(cs => cs.StartDate).WithMessage("Sezon bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz sezon durumu.");
  }
}

public class ChangeCompetitionSeasonStatusRequestValidator : AbstractValidator<ChangeCompetitionSeasonStatusRequest>
{
  public ChangeCompetitionSeasonStatusRequestValidator()
  {
    RuleFor(cs => cs.Id)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz sezon durumu.");
  }
}
