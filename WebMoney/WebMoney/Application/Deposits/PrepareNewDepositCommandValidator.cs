using FluentValidation;

namespace WebMoney.Application.Deposits;

public sealed class PrepareNewDepositCommandValidator : AbstractValidator<PrepareNewDepositCommand>
{
    private const decimal MinAmount = 0.01m;
    private const decimal MaxAmount = 1_000_000_000m;

    public PrepareNewDepositCommandValidator()
    {
        RuleFor(x => x.CardId).GreaterThan(0).WithMessage("Не указана карта.");

        RuleFor(x => x.NormalizedEmail).NotEmpty().WithMessage("Не указан email пользователя.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(MinAmount)
            .WithMessage("Сумма должна быть не меньше 0,01.")
            .LessThanOrEqualTo(MaxAmount)
            .WithMessage("Сумма не может превышать 1 000 000 000.");
    }
}
