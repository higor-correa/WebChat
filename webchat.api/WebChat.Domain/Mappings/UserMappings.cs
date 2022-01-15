using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Mappings;

public static class UserMappings
{
    public static UserDTO ToDto(this User user)
    {
        return new UserDTO
        {
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Id = user.Id,
        };
    }
}
