using FluentAssertions;
using Moq;
using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Application.UseCases.CreateOrder;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Xunit;

namespace Orders.Tests.Unit.Application;

public class CreateOrderHandlerTests
{
    private readonly Mock<IPedidoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_repoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeveCriarPedidoComSucesso()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<ItemPedidoRequest>
            {
                new(Guid.NewGuid(), "Produto Teste", 2, 50.00m)
            }
        );

        _repoMock.Setup(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

        // Act
        var resultado = await _handler.HandleAsync(command);

        // Assert
        resultado.Should().NotBeNull();
        resultado.UsuarioId.Should().Be(command.UsuarioId);
        resultado.Itens.Should().HaveCount(1);
        resultado.ValorTotal.Should().Be(100.00m);

        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
