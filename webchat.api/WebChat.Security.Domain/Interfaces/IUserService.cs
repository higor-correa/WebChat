using WebChat.Security.Domain.DTOs;

namespace WebChat.Security.Domain.Interfaces;

public interface IUserService
{
    Task<UserDTO?> GetUserAsync(string email, string passwordHashed);
}
