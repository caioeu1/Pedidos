using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Orders.Domain.ValueObjects;

namespace Orders.Application.UseCases.CreateOrder;

public class CreateOrderHandler
{
    private readonly IPedidoRepository _repository;
    private readonly IUnitOfWork _uow;

    public CreateOrderHandler(IPedidoRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<PedidoResponse> HandleAsync(CreateOrderCommand cmd, CancellationToken ct = default)
    {
        var pedido = Pedido.Criar(cmd.UsuarioId);

        foreach (var item in cmd.Itens)
        {
            var itemPedido = ItemPedido.Criar(item.ProdutoId, item.NomeProduto, item.Quantidade, item.PrecoUnitario);
            pedido.AdicionarItem(itemPedido);
        }

        pedido.Confirmar();

        await _repository.AdicionarAsync(pedido, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToResponse(pedido);
    }

    private static PedidoResponse MapToResponse(Pedido pedido) =>
        new(
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
