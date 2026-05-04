using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;

namespace WebMoney.Application.Deposits;

public sealed class PrepareNewDepositCommandValidator : AbstractValidator<PrepareNewDepositCommand>
{
    private const decimal MinAmount = 0.01m;
    private const decimal MaxAmount = 1_000_000_000m;

    public PrepareNewDepositCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.CardId).GreaterThan(0).WithMessage(_ => ValidationString.From(localizer, "Validation_CardIdRequired"));

        RuleFor(x => x.UserId).GreaterThan(0).WithMessage(_ => ValidationString.From(localizer, "Validation_UserIdRequired"));

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(MinAmount)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_AmountMin"))
            .LessThanOrEqualTo(MaxAmount)
            .WithMessage(_ => ValidationString.From(localizer, "Validation_AmountMax"));
    }
}