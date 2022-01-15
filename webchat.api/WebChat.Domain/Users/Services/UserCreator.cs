using WebChat.Domain.Employees.Interfaces;
using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;
using WebChat.Domain.Employees.Interfaces;

namespace WebChat.Domain.Users.Services;

public class UserCreator : IUserCreator
{
    private readonly IUserRepository _userRepository;

    public UserCreator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateAsync(CreateUserDTO createUserDTO)
    {
        var user = new User(createUserDTO.Name, createUserDTO.Surname, createUserDTO.Email, createUserDTO.Password);

        await _userRepository.CreateAsync(user);

        return user;
    }
}
