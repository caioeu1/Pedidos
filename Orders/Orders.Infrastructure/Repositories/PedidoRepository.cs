using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly OrdersDbContext _context;

    public PedidoRepository(OrdersDbContext context) => _context = context;

    public async Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Pedidos.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Pedido>> ObterPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        await _context.Pedidos
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync(ct);

    public async Task AdicionarAsync(Pedido pedido, CancellationToken ct = default) =>
        await _context.Pedidos.AddAsync(pedido, ct);

    public Task AtualizarAsync(Pedido pedido, CancellationToken ct = default)
    {
        _context.Pedidos.Update(pedido);
        return Task.CompletedTask;
    }
}
