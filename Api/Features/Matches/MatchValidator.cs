using FluentValidation;

namespace Api.Features.Matches;

public class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
{
  public CreateMatchRequestValidator()
  {
    RuleFor(m => m.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(m => m.HomeTeamId)
      .NotEmpty().WithMessage("Geçersiz ev sahibi takım ID.");

    RuleFor(m => m.AwayTeamId)
      .NotEmpty().WithMessage("Geçersiz deplasman takım ID.");

    RuleFor(m => m.KickoffTime)
      .NotEmpty().WithMessage("Maç başlama saati zorunludur.");

    RuleFor(m => m.Status)
      .IsInEnum().WithMessage("Geçersiz maç durumu.");

    RuleFor(m => m.Round)
      .GreaterThan(0).WithMessage("Hafta numarası 0'dan büyük olmalıdır.")
      .When(m => m.Round.HasValue);

    RuleFor(m => m.Venue)
      .MaximumLength(200).WithMessage("Stadyum adı 200 karakterden uzun olamaz.")
      .When(m => !string.IsNullOrWhiteSpace(m.Venue));
  }
}

public class UpdateMatchRequestValidator : AbstractValidator<UpdateMatchRequest>
{
  public UpdateMatchRequestValidator()
  {
    RuleFor(m => m.Id)
      .NotEmpty().WithMessage("Geçersiz maç ID.");

    RuleFor(m => m.CompetitionSeasonId)
      .NotEmpty().WithMessage("Geçersiz sezon ID.");

    RuleFor(m => m.HomeTeamId)
      .NotEmpty().WithMessage("Geçersiz ev sahibi takım ID.");

    RuleFor(m => m.AwayTeamId)
      .NotEmpty().WithMessage("Geçersiz deplasman takım ID.");

    RuleFor(m => m.KickoffTime)
      .NotEmpty().WithMessage("Maç başlama saati zorunludur.");

    RuleFor(m => m.Round)
      .GreaterThan(0).WithMessage("Hafta numarası 0'dan büyük olmalıdır.")
      .When(m => m.Round.HasValue);

    RuleFor(m => m.Venue)
      .MaximumLength(200).WithMessage("Stadyum adı 200 karakterden uzun olamaz.")
      .When(m => !string.IsNullOrWhiteSpace(m.Venue));
  }
}

public class ChangeMatchStatusRequestValidator : AbstractValidator<ChangeMatchStatusRequest>
{
  public ChangeMatchStatusRequestValidator()
  {
    RuleFor(m => m.Id)
      .NotEmpty().WithMessage("Geçersiz maç ID.");

    RuleFor(m => m.Status)
      .IsInEnum().WithMessage("Geçersiz maç durumu.");
  }
}

public class RecordMatchResultRequestValidator : AbstractValidator<RecordMatchResultRequest>
{
  public RecordMatchResultRequestValidator()
  {
    RuleFor(m => m.Id)
      .NotEmpty().WithMessage("Geçersiz maç ID.");

    RuleFor(m => m.HomeScore)
      .GreaterThanOrEqualTo(0).WithMessage("Ev sahibi skoru negatif olamaz.");

    RuleFor(m => m.AwayScore)
      .GreaterThanOrEqualTo(0).WithMessage("Deplasman skoru negatif olamaz.");
  }
}
