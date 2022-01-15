namespace WebChat.Security.Domain.DTOs;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
}
