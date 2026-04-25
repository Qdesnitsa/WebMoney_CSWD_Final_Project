using MediatR;
using WebMoney.Services;

namespace WebMoney.Application.Auth;

public sealed class SignUpCommandHandler(IAuthService authService)
    : IRequestHandler<SignUpCommand, AuthResult>
{
    public Task<AuthResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = authService.Register(request.UserName, request.Email, request.Password);
        return Task.FromResult(result);
    }
}
