using Orders.Domain.Enums;

namespace Orders.Application.DTOs;

public record PedidoResponse(
    Guid Id,
    Guid UsuarioId,
    DateTime DataPedido,
    OrderStatus Status,
    decimal ValorTotal,
    List<ItemPedidoResponse> Itens);

public record ItemPedidoResponse(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Total);
