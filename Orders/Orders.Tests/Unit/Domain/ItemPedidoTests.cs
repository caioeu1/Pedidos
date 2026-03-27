using FluentAssertions;
using Orders.Domain.ValueObjects;
using Xunit;

namespace Orders.Tests.Unit.Domain;

public class ItemPedidoTests
{
    [Fact]
    public void Criar_DeveCalcularTotalCorretamente()
    {
        // Arrange & Act
        var item = ItemPedido.Criar(Guid.NewGuid(), "Produto Teste", 3, 25.00m);

        // Assert
        item.Total.Should().Be(75.00m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Criar_DeveLancarExcecao_QuandoQuantidadeInvalida(int quantidade)
    {
        // Act
        var acao = () => ItemPedido.Criar(Guid.NewGuid(), "Produto", quantidade, 10m);

        // Assert
        acao.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Criar_DeveLancarExcecao_QuandoPrecoZero()
    {
        // Act
        var acao = () => ItemPedido.Criar(Guid.NewGuid(), "Produto", 1, 0m);

        // Assert
        acao.Should().Throw<ArgumentException>();
    }
}
