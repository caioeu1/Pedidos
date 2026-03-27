using Orders.Application.DTOs;
using Orders.Domain.Interfaces;

namespace Orders.Application.UseCases.GetOrder;

public class GetOrderHandler
{
    private readonly IPedidoRepository _repository;

    public GetOrderHandler(IPedidoRepository repository) => _repository = repository;

    public async Task<PedidoResponse?> HandleAsync(GetOrderQuery query, CancellationToken ct = default)
    {
        var pedido = await _repository.ObterPorIdAsync(query.PedidoId, ct);
        if (pedido is null) return null;

        return new PedidoResponse(
            pedido.Id,
            pedido.UsuarioId,
            pedido.DataPedido,
            pedido.Status,
            pedido.ValorTotal.Valor,
            pedido.Itens.Select(i => new ItemPedidoResponse(
                i.ProdutoId, i.NomeProduto, i.Quantidade, i.PrecoUnitario, i.Total
            )).ToList()
        );
    }
}
