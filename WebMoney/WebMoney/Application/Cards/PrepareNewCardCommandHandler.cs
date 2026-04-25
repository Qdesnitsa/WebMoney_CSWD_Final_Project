using MediatR;
using WebMoney.Services;

namespace WebMoney.Application.Cards;

public class PrepareNewCardCommandHandler(ICardService cardService)
    : IRequestHandler<PrepareNewCardCommand, PrepareNewCardResult>
{
    public Task<PrepareNewCardResult> Handle(PrepareNewCardCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var input = new NewCardInput
        {
            CardNumber = request.CardNumber,
            CurrencyCode = request.CurrencyCode,
            DailyLimit = request.DailyLimit,
            MonthlyLimit = request.MonthlyLimit,
            PerOperationLimit = request.PerOperationLimit,
            PinCode = request.PinCode
        };
        
        var result = cardService.PrepareNewCard(request.NormalizedEmail, input);
        return Task.FromResult(result);
    }
}