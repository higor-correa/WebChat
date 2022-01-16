using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Users.Interfaces;

public interface IUserCreator
{
    Task<User> CreateAsync(CreateUserDTO createUserDTO);
}
