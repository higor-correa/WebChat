using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Employees.Interfaces;

public interface IUserCreator
{
    Task<User> CreateAsync(CreateUserDTO createUserDTO);
}
