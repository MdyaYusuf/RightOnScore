using FluentValidation;

namespace Api.Features.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
  public UpdateUserRequestValidator()
  {
    RuleFor(u => u.Username)
      .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
      .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.");

    RuleFor(u => u.Email)
      .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
      .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
  }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
  public ChangePasswordRequestValidator()
  {
    RuleFor(u => u.CurrentPassword)
      .NotEmpty().WithMessage("Mevcut şifrenizi giriniz.");

    RuleFor(u => u.NewPassword)
      .NotEmpty().WithMessage("Yeni şifre boş olamaz.")
      .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır.")
      .Matches(@"[A-Z]").WithMessage("Yeni şifre en az bir büyük harf içermelidir.")
      .Matches(@"[a-z]").WithMessage("Yeni şifre en az bir küçük harf içermelidir.")
      .Matches(@"[0-9]").WithMessage("Yeni şifre en az bir rakam içermelidir.")
      .NotEqual(u => u.CurrentPassword).WithMessage("Yeni şifre mevcut şifre ile aynı olamaz.");

    RuleFor(u => u.ConfirmNewPassword)
      .Equal(u => u.NewPassword).WithMessage("Şifreler eşleşmiyor.");
  }
}