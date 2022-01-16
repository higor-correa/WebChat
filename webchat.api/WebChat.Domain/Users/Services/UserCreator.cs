using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;
using WebChat.Domain.Users.Interfaces;

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
        var user = new User(createUserDTO.Username, createUserDTO.Password);

        await _userRepository.CreateAsync(user);

        return user;
    }
}
