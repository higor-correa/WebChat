using WebChat.Domain.Users.Entities;
using WebChat.Domain.Users.Interfaces;

namespace WebChat.Domain.Users.Services;

public class UserSearcher : IUserSearcher
{
    public IUserRepository _userRepository;

    public UserSearcher(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User?> GetForLoginAsync(string email, string passwordHashed)
    {
        return _userRepository.GetForLoginAsync(email, passwordHashed);
    }
}
