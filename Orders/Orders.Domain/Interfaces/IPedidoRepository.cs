using Orders.Domain.Entities;

namespace Orders.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Pedido>> ObterPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken ct = default);
    Task AtualizarAsync(Pedido pedido, CancellationToken ct = default);
}
