using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Security;

public interface ILoginFacade
{
    Task<string> AuthenticateAsync(LoginDTO loginDTO);
}