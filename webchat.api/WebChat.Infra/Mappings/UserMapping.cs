using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography;
using System.Text;
using WebChat.Domain.Users.Entities;

namespace WebChat.Infra.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    private const string AdminId = "661b8028-6ce0-4544-950d-18837c2bcd7e";
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Surname).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.PasswordHash);


        builder.HasData(new User("Admin", string.Empty, "admin@admin.com", GetPassword())
        {
            Id = Guid.Parse(AdminId)
        });
    }

    private static string GetPassword()
    {
        return SHA256.Create()
                     .ComputeHash(Encoding.UTF8.GetBytes("admin"))
                     .Select(x => string.Format("{0:x2}", x))
                     .Aggregate((@byte, hash) => hash + @byte);
    }
}
