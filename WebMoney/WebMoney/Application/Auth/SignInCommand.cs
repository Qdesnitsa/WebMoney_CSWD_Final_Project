using MediatR;

namespace WebMoney.Application.Auth;

public sealed record SignInCommand(
    string Email, 
    string Password) : IRequest<AuthResult>;
