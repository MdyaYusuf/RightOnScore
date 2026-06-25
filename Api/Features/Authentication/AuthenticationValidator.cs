using FluentValidation;

namespace Api.Features.Authentication;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(u => u.Email)
      .NotEmpty().WithMessage("E-posta adresi gereklidir.")
      .EmailAddress().WithMessage("Geçerli bir e-posta formatı giriniz.");

    RuleFor(u => u.Password)
      .NotEmpty().WithMessage("Şifre gereklidir.");
  }
}

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
  public RegisterUserRequestValidator()
  {
    RuleFor(u => u.Username)
      .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
      .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
      .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.");

    RuleFor(u => u.Email)
      .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
      .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

    RuleFor(u => u.Password)
      .NotEmpty().WithMessage("Şifre boş olamaz.")
      .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
      .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
      .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
      .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");
  }
}