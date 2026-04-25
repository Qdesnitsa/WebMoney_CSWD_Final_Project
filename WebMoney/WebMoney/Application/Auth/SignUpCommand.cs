using MediatR;

namespace WebMoney.Application.Auth;

public sealed record SignUpCommand(
    string UserName, 
    string Email, 
    string Password, 
    string ConfirmPassword) : IRequest<AuthResult>;
