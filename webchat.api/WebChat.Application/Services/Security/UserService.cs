using WebChat.Domain.Users.Interfaces;
using WebChat.Security.Domain.DTOs;
using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Security;

public class UserService : IUserService
{
    private readonly IUserSearcher _userSearcher;

    public UserService(IUserSearcher UserSearcher)
    {
        _userSearcher = UserSearcher;
    }

    public async Task<UserDTO?> GetUserAsync(string email, string passwordHashed)
    {
        var user = await _userSearcher.GetForLoginAsync(email, passwordHashed);

        return user == null 
            ? default
            : new()
            {
                Id = user.Id,
                Username = user.Username,
            };
    }
}
