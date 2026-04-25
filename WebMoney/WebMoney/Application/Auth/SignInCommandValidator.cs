using FluentValidation;

namespace WebMoney.Application.Auth;

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    public SignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Укажите email")
            .EmailAddress().WithMessage("Введите корректный адрес email")
            .Length(5, 256).WithMessage("Email: 5–256 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Укажите пароль")
            .MinimumLength(8).WithMessage("Пароль: минимум 8 символов")
            .MaximumLength(100).WithMessage("Пароль: максимум 100 символов")
            .Matches(PasswordPattern)
            .WithMessage("Пароль: латиница, цифра, заглавная и строчная буквы, от 8 символов");
    }
}
