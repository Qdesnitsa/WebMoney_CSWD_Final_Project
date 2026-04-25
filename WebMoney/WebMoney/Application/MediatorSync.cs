using MediatR;

namespace WebMoney.Application;

public static class MediatorSync
{
    public static TResponse SendSync<TResponse>(this IMediator mediator, IRequest<TResponse> request) =>
        mediator.Send(request, CancellationToken.None).GetAwaiter().GetResult();
}
