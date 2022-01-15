using WebChat.Domain.Users.DTOs;

namespace WebChat.Application.Services.Users;

public interface IUserFacade
{
    Task<UserDTO> CreateAsync(CreateUserDTO userDTO);
}
