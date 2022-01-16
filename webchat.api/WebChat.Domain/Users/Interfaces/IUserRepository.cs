using WebChat.Domain.Interfaces;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Users.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetForLoginAsync(string email, string passwordHashed);
}
