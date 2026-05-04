using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;

namespace WebMoney.Application.Auth;

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
    private const string UserNamePattern = @"^[a-zA-Z0-9_-]+$";

    public SignUpCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_UserNameRequired"))
            .Length(3, 256).WithMessage(_ => ValidationString.From(localizer, "Validation_UserNameLength"))
            .Matches(UserNamePattern)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_UserNamePattern"));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_EmailRequired"))
            .EmailAddress().WithMessage(_ => ValidationString.From(localizer, "Validation_EmailInvalid"))
            .Length(5, 256).WithMessage(_ => ValidationString.From(localizer, "Validation_EmailLength"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_PasswordRequired"))
            .MinimumLength(8).WithMessage(_ => ValidationString.From(localizer, "Validation_PasswordMinLength"))
            .MaximumLength(100).WithMessage(_ => ValidationString.From(localizer, "Validation_PasswordMaxLength"))
            .Matches(PasswordPattern)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_PasswordPattern"));

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_ConfirmPasswordRequired"))
            .Equal(x => x.Password).WithMessage(_ => ValidationString.From(localizer, "Validation_ConfirmPasswordMismatch"));
    }
}
