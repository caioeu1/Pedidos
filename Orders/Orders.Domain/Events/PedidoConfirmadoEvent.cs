namespace Orders.Domain.Events;

public record PedidoConfirmadoEvent(Guid PedidoId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
