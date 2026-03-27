using Orders.Application.Interfaces;
using Orders.Domain.Enums;
using Orders.Domain.Interfaces;

namespace Orders.Application.UseCases.UpdateOrderStatus;

public class UpdateOrderStatusHandler
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateOrderStatusHandler(IPedidoRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task HandleAsync(UpdateOrderStatusCommand cmd, CancellationToken ct = default)
    {
        var pedido = await _repository.ObterPorIdAsync(cmd.PedidoId, ct)
            ?? throw new KeyNotFoundException($"Pedido {cmd.PedidoId} não encontrado.");

        if (cmd.NovoStatus == OrderStatus.Cancelled)
            pedido.Cancelar();

        await _repository.AtualizarAsync(pedido, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
