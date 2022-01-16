using Microsoft.EntityFrameworkCore;
using WebChat.Domain.Users.Entities;
using WebChat.Domain.Users.Interfaces;

namespace WebChat.Infra.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(WebChatContext employeeContext) : base(employeeContext)
    {
    }

    public Task<User?> GetForLoginAsync(string email, string passwordHashed)
    {
        return Set.Where(x => x.Username == email && x.PasswordHash == passwordHashed).FirstOrDefaultAsync();
    }
}
