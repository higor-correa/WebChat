namespace WebChat.Security.Domain.Interfaces;

public interface ILoginService
{
    Task<string> AuthenticateAsync(LoginDTO loginDTO);
}