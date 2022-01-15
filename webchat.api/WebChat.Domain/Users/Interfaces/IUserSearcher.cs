using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Users.Interfaces;

public interface IUserSearcher
{
    Task<User?> GetForLoginAsync(string email, string passwordHashed);
}
