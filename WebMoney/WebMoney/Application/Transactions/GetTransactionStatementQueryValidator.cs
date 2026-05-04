using FluentValidation;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;

namespace WebMoney.Application.Transactions;

public sealed class GetTransactionStatementQueryValidator : AbstractValidator<GetTransactionStatementQuery>
{
    public GetTransactionStatementQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.CardId).GreaterThan(0).WithMessage(_ => ValidationString.From(localizer, "Validation_CardIdRequired"));

        When(x => x.PeriodKeysPresentInQuery, () =>
        {
            RuleFor(x => x.PeriodFrom)
                .NotNull().WithMessage(_ => ValidationString.From(localizer, "Validation_PeriodFromRequired"));
            RuleFor(x => x.PeriodTo)
                .NotNull().WithMessage(_ => ValidationString.From(localizer, "Validation_PeriodToRequired"));
        });

        When(x => x.PeriodFrom.HasValue && x.PeriodTo.HasValue, () =>
        {
            RuleFor(x => x.PeriodFrom)
                .LessThanOrEqualTo(x => x.PeriodTo!.Value)
                .WithMessage(_ => ValidationString.From(localizer, "Validation_PeriodRangeInvalid"));
        });
    }
}
