namespace WebMoney.Services;

public interface IAuthService
{
    AuthResult TrySignIn(string email, string password);
    AuthResult Register(string username, string email, string password);
}