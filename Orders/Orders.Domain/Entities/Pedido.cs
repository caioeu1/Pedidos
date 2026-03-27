using Orders.Domain.Enums;
using Orders.Domain.Events;
using Orders.Domain.Exceptions;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Entities;

public class Pedido
{
    private List<ItemPedido> _itens = new();

    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime DataPedido { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<ItemPedido> Itens => _itens.AsReadOnly();
    public Dinheiro ValorTotal => _itens
        .Select(i => new Dinheiro(i.Total))
        .Aggregate(Dinheiro.Zero, (acc, d) => acc.Somar(d));

    private readonly List<IDomainEvent> _eventos = new();
    public IReadOnlyList<IDomainEvent> Eventos => _eventos.AsReadOnly();

    private Pedido() { }

    public static Pedido Criar(Guid usuarioId)
    {
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            DataPedido = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        pedido._eventos.Add(new PedidoCriadoEvent(pedido.Id, usuarioId));
        return pedido;
    }

    public void AdicionarItem(ItemPedido item)
    {
        if (Status != OrderStatus.Pending)
            throw new PedidoInvalidoException("Só é possível adicionar itens a pedidos pendentes.");
        _itens.Add(item);
    }

    public void Confirmar()
    {
        if (!_itens.Any())
            throw new PedidoInvalidoException("Um pedido não pode ser confirmado sem itens.");
        Status = OrderStatus.Confirmed;
        _eventos.Add(new PedidoConfirmadoEvent(Id));
    }

    public void Cancelar()
    {
        if (Status == OrderStatus.Delivered)
            throw new PedidoInvalidoException("Pedidos entregues não podem ser cancelados.");
        Status = OrderStatus.Cancelled;
    }

    public void LimparEventos() => _eventos.Clear();
}
