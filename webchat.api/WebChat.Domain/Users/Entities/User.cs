
namespace WebChat.Domain.Users.Entities;
public class User : EntityBase
{
    public User() { }

    public User(string name, string surname, string email, string passwordHash)
    {
        Name = name;
        Surname = surname;
        Email = email;
        PasswordHash = passwordHash ?? string.Empty;
    }

    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}
