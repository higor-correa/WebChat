
namespace WebChat.Domain.Users.Entities;
public class User : EntityBase
{
    public User() { }

    public User(string email, string passwordHash)
    {
        Username = email;
        PasswordHash = passwordHash ?? string.Empty;
    }

    public string Username { get; set; }
    public string PasswordHash { get; set; }
}
