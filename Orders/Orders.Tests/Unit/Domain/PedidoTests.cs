using FluentAssertions;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;
using Orders.Domain.ValueObjects;
using Xunit;

namespace Orders.Tests.Unit.Domain;

public class PedidoTests
{
    [Fact]
    public void Criar_DeveCriarPedidoComStatusPendente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var pedido = Pedido.Criar(usuarioId);

        // Assert
        pedido.Id.Should().NotBeEmpty();
        pedido.UsuarioId.Should().Be(usuarioId);
        pedido.Status.Should().Be(OrderStatus.Pending);
        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Confirmar_DeveConfirmarPedidoComItens()
    {
        // Arrange
        var pedido = Pedido.Criar(Guid.NewGuid());
        var item = ItemPedido.Criar(Guid.NewGuid(), "Produto A", 2, 50m);
        pedido.AdicionarItem(item);

        // Act
        pedido.Confirmar();

        // Assert
        pedido.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirmar_DeveLancarExcecao_QuandoPedidoSemItens()
    {
        // Arrange
        var pedido = Pedido.Criar(Guid.NewGuid());

        // Act
        var acao = () => pedido.Confirmar();

        // Assert
        acao.Should().Throw<PedidoInvalidoException>()
            .WithMessage("*sem itens*");
    }

    [Fact]
    public void ValorTotal_DeveCalcularCorretamente()
    {
        // Arrange
        var pedido = Pedido.Criar(Guid.NewGuid());
        pedido.AdicionarItem(ItemPedido.Criar(Guid.NewGuid(), "Prod A", 2, 100m));
        pedido.AdicionarItem(ItemPedido.Criar(Guid.NewGuid(), "Prod B", 1, 50m));

        // Act
        var total = pedido.ValorTotal;

        // Assert
        total.Valor.Should().Be(250m);
    }

    [Fact]
    public void Cancelar_DeveLancarExcecao_QuandoPedidoJaEntregue()
    {
        // Arrange - forçamos status via reflexão para simular
        var pedido = Pedido.Criar(Guid.NewGuid());
        var statusProp = typeof(Pedido).GetProperty("Status");
        statusProp!.SetValue(pedido, OrderStatus.Delivered);

        // Act
        var acao = () => pedido.Cancelar();

        // Assert
        acao.Should().Throw<PedidoInvalidoException>();
    }
}
