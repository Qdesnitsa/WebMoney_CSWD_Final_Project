using FluentValidation;

namespace WebMoney.Application.Cards;

public sealed class PrepareNewCardCommandValidator : AbstractValidator<PrepareNewCardCommand>
{
    private const int CardNumberLength = 16;

    public PrepareNewCardCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Пользователь не указан.");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(CardNumberLength)
            .Matches(@"^[1-9]\d{15}$")
            .WithMessage("Номер карты — 16 цифр, не начинается с 0");

        RuleFor(x => x.PinCode)
            .NotEmpty()
            .Matches(@"^\d{4}$")
            .WithMessage("PIN-код — ровно 4 цифры");

        RuleFor(x => x.CurrencyCode).IsInEnum().WithMessage("Выберите валюту.");

        RuleFor(x => x.DailyLimit)
            .GreaterThan(0)
            .When(x => x.DailyLimit.HasValue)
            .WithMessage("Дневной лимит должен быть больше 0");

        RuleFor(x => x.MonthlyLimit)
            .GreaterThan(0)
            .When(x => x.MonthlyLimit.HasValue)
            .WithMessage("Месячный лимит должен быть больше 0");

        RuleFor(x => x.PerOperationLimit)
            .GreaterThan(0)
            .When(x => x.PerOperationLimit.HasValue)
            .WithMessage("Лимит на одну операцию должен быть больше 0");
    }
}
