using WebChat.Domain.Users.DTOs;
using WebChat.Domain.Users.Entities;

namespace WebChat.Domain.Mappings;

public static class UserMappings
{
    public static UserDTO ToDto(this User user)
    {
        return new UserDTO
        {
            Username = user.Username,
            Id = user.Id,
        };
    }
}
