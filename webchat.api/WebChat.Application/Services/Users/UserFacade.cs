using WebChat.Domain.Employees.Interfaces;
using WebChat.Domain.Mappings;
using WebChat.Domain.Users.DTOs;
using WebChat.Security.Domain.Interfaces;

namespace WebChat.Application.Services.Users;

public class UserFacade : IUserFacade
{
    private readonly IUserCreator _userCreator;
    private readonly IPasswordHasher _passwordHasher;
    

    public UserFacade(IUserCreator userCreator)
    {
        _userCreator = userCreator;
    }

    public async Task<UserDTO> CreateAsync(CreateUserDTO userDTO)
    {
        userDTO.Password = _passwordHasher.HashPassword(userDTO.Password);
        var employee = await _userCreator.CreateAsync(userDTO);
        return employee.ToDto();
    }
}
