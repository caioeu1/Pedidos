using Orders.Domain.Enums;

namespace Orders.Application.UseCases.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid PedidoId, OrderStatus NovoStatus);
