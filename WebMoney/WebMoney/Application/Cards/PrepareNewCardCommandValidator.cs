using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;

namespace WebMoney.Application.Cards;

public sealed class PrepareNewCardCommandValidator : AbstractValidator<PrepareNewCardCommand>
{
    private const int CardNumberLength = 16;

    public PrepareNewCardCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage(_ => ValidationString.From(localizer, "Validation_PrepareCardUserRequired"));

        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_CardNumberEmpty"))
            .Length(CardNumberLength).WithMessage(_ => ValidationString.From(localizer, "Validation_CardNumberLength"))
            .Matches(@"^[1-9]\d{15}$")
            .WithMessage(_ => ValidationString.From(localizer, "Validation_CardNumberFormat"));

        RuleFor(x => x.PinCode)
            .NotEmpty().WithMessage(_ => ValidationString.From(localizer, "Validation_PinRequired"))
            .Matches(@"^\d{4}$")
            .WithMessage(_ => ValidationString.From(localizer, "Validation_PinFormat"));

        RuleFor(x => x.CurrencyCode).IsInEnum().WithMessage(_ => ValidationString.From(localizer, "Validation_CurrencyRequired"));

        RuleFor(x => x.DailyLimit)
            .GreaterThan(0)
            .When(x => x.DailyLimit.HasValue)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_LimitDailyPositive"));

        RuleFor(x => x.MonthlyLimit)
            .GreaterThan(0)
            .When(x => x.MonthlyLimit.HasValue)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_LimitMonthlyPositive"));

        RuleFor(x => x.PerOperationLimit)
            .GreaterThan(0)
            .When(x => x.PerOperationLimit.HasValue)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_LimitPerOperationPositive"));
    }
}