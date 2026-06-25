using FluentValidation;

namespace Api.Features.CompetitionStages;

public class CreateCompetitionStageRequestValidator : AbstractValidator<CreateCompetitionStageRequest>
{
  public CreateCompetitionStageRequestValidator()
  {
    RuleFor(cs => cs.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(cs => cs.Name)
      .NotEmpty().WithMessage("Aşama adı boş olamaz.")
      .NotNull().WithMessage("Aşama adı zorunludur.")
      .MinimumLength(2).WithMessage("Aşama adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Aşama adı 100 karakterden uzun olamaz.");

    RuleFor(cs => cs.Type)
      .IsInEnum().WithMessage("Geçersiz aşama tipi.");

    RuleFor(cs => cs.DisplayOrder)
      .GreaterThan(0).WithMessage("Aşama sırası 0'dan büyük olmalıdır.");

    RuleFor(cs => cs.StartDate)
      .NotEmpty().WithMessage("Aşama başlangıç tarihi zorunludur.");

    RuleFor(cs => cs.EndDate)
      .NotEmpty().WithMessage("Aşama bitiş tarihi zorunludur.")
      .GreaterThan(cs => cs.StartDate).WithMessage("Aşama bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz aşama durumu.");
  }
}

public class UpdateCompetitionStageRequestValidator : AbstractValidator<UpdateCompetitionStageRequest>
{
  public UpdateCompetitionStageRequestValidator()
  {
    RuleFor(cs => cs.Id)
      .NotEmpty().WithMessage("Geçersiz aşama ID.");

    RuleFor(cs => cs.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(cs => cs.Name)
      .NotEmpty().WithMessage("Aşama adı boş olamaz.")
      .MinimumLength(2).WithMessage("Aşama adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Aşama adı 100 karakterden uzun olamaz.");

    RuleFor(cs => cs.Type)
      .IsInEnum().WithMessage("Geçersiz aşama tipi.");

    RuleFor(cs => cs.DisplayOrder)
      .GreaterThan(0).WithMessage("Aşama sırası 0'dan büyük olmalıdır.");

    RuleFor(cs => cs.StartDate)
      .NotEmpty().WithMessage("Aşama başlangıç tarihi zorunludur.");

    RuleFor(cs => cs.EndDate)
      .NotEmpty().WithMessage("Aşama bitiş tarihi zorunludur.")
      .GreaterThan(cs => cs.StartDate).WithMessage("Aşama bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz aşama durumu.");
  }
}

public class ChangeCompetitionStageStatusRequestValidator : AbstractValidator<ChangeCompetitionStageStatusRequest>
{
  public ChangeCompetitionStageStatusRequestValidator()
  {
    RuleFor(cs => cs.Id)
      .NotEmpty().WithMessage("Geçersiz aşama ID.");

    RuleFor(cs => cs.Status)
      .IsInEnum().WithMessage("Geçersiz aşama durumu.");
  }
}
