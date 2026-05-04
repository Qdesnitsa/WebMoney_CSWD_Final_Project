using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using WebMoney;
using WebMoney.Localization;
using WebMoney.Models;

namespace WebMoney.Validators;

public sealed class TransactionViewModelValidator : AbstractValidator<TransactionViewModel>
{
    public TransactionViewModelValidator(IStringLocalizer<SharedResource> localizer, IHttpContextAccessor http)
    {
        When(_ => PeriodKeysPresentInRequest(http), () =>
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

    private static bool PeriodKeysPresentInRequest(IHttpContextAccessor http)
    {
        var query = http.HttpContext?.Request.Query;
        if (query is null || query.Count == 0)
        {
            return false;
        }

        return query.Keys.Any(static k =>
            string.Equals(k, "PeriodFrom", StringComparison.OrdinalIgnoreCase)
            || string.Equals(k, "PeriodTo", StringComparison.OrdinalIgnoreCase));
    }
}
