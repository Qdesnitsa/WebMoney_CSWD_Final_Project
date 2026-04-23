using FluentValidation;
using WebMoney.Models;

namespace WebMoney.Application.Auth;

public sealed class SignUpViewModelValidator : AbstractValidator<SignUpViewModel>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
    private const string UserNamePattern = @"^[a-zA-Z0-9_-]+$";

    public SignUpViewModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Укажите ваше имя")
            .Length(3, 256).WithMessage("Имя: 3–256 символов")
            .Matches(UserNamePattern)
            .WithMessage("Только латинские буквы, цифры, _ и -");

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

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Подтвердите пароль")
            .Equal(x => x.Password).WithMessage("Пароли не совпадают");
    }
}
