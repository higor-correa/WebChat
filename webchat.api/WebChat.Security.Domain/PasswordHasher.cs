using System.Security.Cryptography;
using System.Text;
using WebChat.Security.Domain.Interfaces;

namespace WebChat.Security.Domain;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return SHA256.Create()
                     .ComputeHash(Encoding.UTF8.GetBytes(password))
                     .Select(x => string.Format("{0:x2}", x))
                     .Aggregate((@byte, hash) => hash + @byte);
    }
}
