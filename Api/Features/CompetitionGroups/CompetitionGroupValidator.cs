using FluentValidation;

namespace Api.Features.CompetitionGroups;

public class CreateCompetitionGroupRequestValidator : AbstractValidator<CreateCompetitionGroupRequest>
{
  public CreateCompetitionGroupRequestValidator()
  {
    RuleFor(cg => cg.CompetitionStageId)
      .NotEmpty().WithMessage("Geçersiz aşama ID.");

    RuleFor(cg => cg.Name)
      .NotEmpty().WithMessage("Grup adı boş olamaz.")
      .NotNull().WithMessage("Grup adı zorunludur.")
      .MinimumLength(1).WithMessage("Grup adı en az 1 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Grup adı 100 karakterden uzun olamaz.");

    RuleFor(cg => cg.DisplayOrder)
      .GreaterThan(0).WithMessage("Grup sırası 0'dan büyük olmalıdır.");
  }
}

public class UpdateCompetitionGroupRequestValidator : AbstractValidator<UpdateCompetitionGroupRequest>
{
  public UpdateCompetitionGroupRequestValidator()
  {
    RuleFor(cg => cg.Id)
      .NotEmpty().WithMessage("Geçersiz grup ID.");

    RuleFor(cg => cg.CompetitionStageId)
      .NotEmpty().WithMessage("Geçersiz aşama ID.");

    RuleFor(cg => cg.Name)
      .NotEmpty().WithMessage("Grup adı boş olamaz.")
      .MinimumLength(1).WithMessage("Grup adı en az 1 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Grup adı 100 karakterden uzun olamaz.");

    RuleFor(cg => cg.DisplayOrder)
      .GreaterThan(0).WithMessage("Grup sırası 0'dan büyük olmalıdır.");
  }
}

public class ChangeCompetitionGroupStatusRequestValidator : AbstractValidator<ChangeCompetitionGroupStatusRequest>
{
  public ChangeCompetitionGroupStatusRequestValidator()
  {
    RuleFor(cg => cg.Id)
      .NotEmpty().WithMessage("Geçersiz grup ID.");
  }
}
