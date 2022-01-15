using Microsoft.EntityFrameworkCore;
using WebChat.Infra.Mappings;

namespace WebChat.Infra;

public class WebChatContext : DbContext
{
    protected WebChatContext() { }

    public WebChatContext(DbContextOptions<WebChatContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapping());

        base.OnModelCreating(modelBuilder);
    }
}
