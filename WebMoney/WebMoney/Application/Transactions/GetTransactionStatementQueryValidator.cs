using FluentValidation;

namespace WebMoney.Application.Transactions;

public sealed class GetTransactionStatementQueryValidator : AbstractValidator<GetTransactionStatementQuery>
{
    public GetTransactionStatementQueryValidator()
    {
        RuleFor(x => x.CardId).GreaterThan(0).WithMessage("Не указана карта.");

        When(x => x.PeriodKeysPresentInQuery, () =>
        {
            RuleFor(x => x.PeriodFrom)
                .NotNull().WithMessage("Укажите дату «с».");
            RuleFor(x => x.PeriodTo)
                .NotNull().WithMessage("Укажите дату «по».");
        });

        When(x => x.PeriodFrom.HasValue && x.PeriodTo.HasValue, () =>
        {
            RuleFor(x => x.PeriodFrom)
                .LessThanOrEqualTo(x => x.PeriodTo!.Value)
                .WithMessage("Дата «с» не может быть позже даты «по».");
        });
    }
}