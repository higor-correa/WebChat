using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Security;

public class LoginFacade : ILoginFacade
{
    private readonly ILoginService _loginService;

    public LoginFacade(ILoginService loginService)
    {
        _loginService = loginService;
    }

    public Task<string> AuthenticateAsync(LoginDTO loginDTO)
    {
        return _loginService.AuthenticateAsync(loginDTO);
    }
}
