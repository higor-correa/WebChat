using WebChat.Domain.Interfaces;

namespace WebChat.Domain;

public abstract class EntityBase : IEntity
{
    public EntityBase() { }

    public Guid Id { get; set; }
}
