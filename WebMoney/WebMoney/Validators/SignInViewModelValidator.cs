using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.LocalizationHelper;
using WebMoney.Models;

namespace WebMoney.Validators;

public sealed class SignInViewModelValidator : AbstractValidator<SignInViewModel>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    public SignInViewModelValidator(IStringLocalizer<SharedResource> localizer)
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
