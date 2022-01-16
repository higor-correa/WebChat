using WebChat.Domain.Mappings;
using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Interfaces;
using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Users;

public class UserFacade : IUserFacade
{
    private readonly IUserCreator _userCreator;
    private readonly IPasswordHasher _passwordHasher;


    public UserFacade(IUserCreator userCreator, IPasswordHasher passwordHasher)
    {
        _userCreator = userCreator;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDTO> CreateAsync(CreateUserDTO userDTO)
    {
        userDTO.Password = _passwordHasher.HashPassword(userDTO.Password);
        var employee = await _userCreator.CreateAsync(userDTO);
        return employee.ToDto();
    }
}
