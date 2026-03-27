namespace Orders.Domain.Events;

public record PedidoCriadoEvent(Guid PedidoId, Guid UsuarioId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
