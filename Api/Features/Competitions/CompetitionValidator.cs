using FluentValidation;

namespace Api.Features.Competitions;

public class CreateCompetitionRequestValidator : AbstractValidator<CreateCompetitionRequest>
{
  public CreateCompetitionRequestValidator()
  {
    RuleFor(c => c.Name)
      .NotEmpty().WithMessage("Yarışma adı boş olamaz.")
      .NotNull().WithMessage("Yarışma adı zorunludur.")
      .MinimumLength(2).WithMessage("Yarışma adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Yarışma adı 100 karakterden uzun olamaz.");

    RuleFor(c => c.Country)
      .NotEmpty().WithMessage("Yarışma ülkesi veya bölgesi boş olamaz.")
      .NotNull().WithMessage("Yarışma ülkesi veya bölgesi zorunludur.")
      .MinimumLength(2).WithMessage("Yarışma ülkesi veya bölgesi en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Yarışma ülkesi veya bölgesi 100 karakterden uzun olamaz.");

    RuleFor(c => c.Type)
      .IsInEnum().WithMessage("Geçersiz yarışma tipi.");

    RuleFor(c => c.LogoUrl)
      .MaximumLength(500).WithMessage("Logo bağlantısı 500 karakterden uzun olamaz.")
      .Must(BeValidUrl).WithMessage("Logo bağlantısı geçerli bir URL olmalıdır.")
      .When(c => !string.IsNullOrWhiteSpace(c.LogoUrl));
  }

  private static bool BeValidUrl(string? logoUrl)
  {
    return Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri) &&
      (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
  }
}

public class UpdateCompetitionRequestValidator : AbstractValidator<UpdateCompetitionRequest>
{
  public UpdateCompetitionRequestValidator()
  {
    RuleFor(c => c.Id)
      .NotEmpty().WithMessage("Geçersiz yarışma ID.");

    RuleFor(c => c.Name)
      .NotEmpty().WithMessage("Yarışma adı boş olamaz.")
      .MinimumLength(2).WithMessage("Yarışma adı en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Yarışma adı 100 karakterden uzun olamaz.");

    RuleFor(c => c.Country)
      .NotEmpty().WithMessage("Yarışma ülkesi veya bölgesi boş olamaz.")
      .MinimumLength(2).WithMessage("Yarışma ülkesi veya bölgesi en az 2 karakter olmalıdır.")
      .MaximumLength(100).WithMessage("Yarışma ülkesi veya bölgesi 100 karakterden uzun olamaz.");

    RuleFor(c => c.Type)
      .IsInEnum().WithMessage("Geçersiz yarışma tipi.");

    RuleFor(c => c.LogoUrl)
      .MaximumLength(500).WithMessage("Logo bağlantısı 500 karakterden uzun olamaz.")
      .Must(BeValidUrl).WithMessage("Logo bağlantısı geçerli bir URL olmalıdır.")
      .When(c => !string.IsNullOrWhiteSpace(c.LogoUrl));
  }

  private static bool BeValidUrl(string? logoUrl)
  {
    return Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri) &&
      (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
  }
}

public class ChangeCompetitionStatusRequestValidator : AbstractValidator<ChangeCompetitionStatusRequest>
{
  public ChangeCompetitionStatusRequestValidator()
  {
    RuleFor(c => c.Id)
      .NotEmpty().WithMessage("Geçersiz yarışma ID.");
  }
}
