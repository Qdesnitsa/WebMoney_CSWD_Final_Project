using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;

namespace WebMoney.Application.Auth;

public sealed class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    public SignInCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
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
    }
}
