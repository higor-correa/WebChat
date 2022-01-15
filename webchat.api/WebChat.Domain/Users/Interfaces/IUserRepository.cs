using WebChat.Domain.Interfaces;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Employees.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetForLoginAsync(string email, string passwordHashed);
}
