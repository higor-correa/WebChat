using Microsoft.EntityFrameworkCore;
using WebChat.Domain.Employees.Interfaces;
using WebChat.Domain.Users.Entities;

namespace WebChat.Infra.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(WebChatContext employeeContext) : base(employeeContext)
    {
    }

    public Task<User?> GetForLoginAsync(string email, string passwordHashed)
    {
        return Set.Where(x => x.Email == email && x.PasswordHash == passwordHashed).FirstOrDefaultAsync();
    }
}
