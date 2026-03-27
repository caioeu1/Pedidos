using Orders.Application.DTOs;

namespace Orders.Application.UseCases.CreateOrder;

public record CreateOrderCommand(Guid UsuarioId, List<ItemPedidoRequest> Itens);
