using FluentValidation;

namespace Api.Features.MatchPredictions;

public class CreateMatchPredictionRequestValidator : AbstractValidator<CreateMatchPredictionRequest>
{
  public CreateMatchPredictionRequestValidator()
  {
    RuleFor(p => p.MatchId)
      .NotEmpty().WithMessage("Geçersiz maç ID.");

    RuleFor(p => p.PredictedHomeScore)
      .GreaterThanOrEqualTo(0).WithMessage("Ev sahibi tahmin skoru negatif olamaz.");

    RuleFor(p => p.PredictedAwayScore)
      .GreaterThanOrEqualTo(0).WithMessage("Deplasman tahmin skoru negatif olamaz.");
  }
}

public class UpdateMatchPredictionRequestValidator : AbstractValidator<UpdateMatchPredictionRequest>
{
  public UpdateMatchPredictionRequestValidator()
  {
    RuleFor(p => p.Id)
      .NotEmpty().WithMessage("Geçersiz skor tahmini ID.");

    RuleFor(p => p.PredictedHomeScore)
      .GreaterThanOrEqualTo(0).WithMessage("Ev sahibi tahmin skoru negatif olamaz.");

    RuleFor(p => p.PredictedAwayScore)
      .GreaterThanOrEqualTo(0).WithMessage("Deplasman tahmin skoru negatif olamaz.");
  }
}
