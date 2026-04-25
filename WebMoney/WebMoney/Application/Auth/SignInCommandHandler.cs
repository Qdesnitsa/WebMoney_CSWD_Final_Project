using MediatR;
using WebMoney.Services;

namespace WebMoney.Application.Auth;

public sealed class SignInCommandHandler(IAuthService authService)
    : IRequestHandler<SignInCommand, AuthResult>
{
    public Task<AuthResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = authService.TrySignIn(request.Email, request.Password);
        return Task.FromResult(result);
    }
}
