using WebChat.Domain.Users.Interfaces;
using WebChat.Security.Domain.DTOs;
using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Security;

public class UserService : IUserService
{
    private readonly IUserSearcher _UserSearcher;

    public UserService(IUserSearcher UserSearcher)
    {
        _UserSearcher = UserSearcher;
    }

    public async Task<UserDTO?> GetUserAsync(string email, string passwordHashed)
    {
        var user = await _UserSearcher.GetForLoginAsync(email, passwordHashed);

        return user == null 
            ? default
            : new()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
            };
    }
}
