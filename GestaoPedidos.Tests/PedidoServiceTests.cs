using Xunit;
using Moq;
using FluentAssertions;
using System;

// ──────────────────────────────────────────────
// Interfaces e classes de domínio
// (se já existirem no GestaoPedidos.API, remova
//  estas definições e use as do projeto da API)
// ──────────────────────────────────────────────

public interface IProdutoRepository
{
    Produto ObterPorId(int produtoId);
}

public interface IEstoqueService
{
    void DebitarEstoque(int produtoId, int quantidade);
}

public class Produto
{
    public int     Id      { get; set; }
    public string  Nome    { get; set; }
    public decimal Preco   { get; set; }
    public int     Estoque { get; set; }
}

// ──────────────────────────────────────────────
// Serviço que será testado
// ──────────────────────────────────────────────

public class PedidoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IEstoqueService    _estoqueService;

    public PedidoService(
        IProdutoRepository produtoRepository,
        IEstoqueService    estoqueService)
    {
        _produtoRepository = produtoRepository;
        _estoqueService    = estoqueService;
    }

    public int CriarPedido(int produtoId, int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        var produto = _produtoRepository.ObterPorId(produtoId);

        if (produto == null)
            throw new ProdutoNaoEncontradoException(
                $"Produto com ID {produtoId} não encontrado.");

        if (produto.Estoque < quantidade)
            throw new EstoqueInsuficienteException(
                $"Estoque insuficiente para o produto {produto.Nome}.");

        _estoqueService.DebitarEstoque(produtoId, quantidade);

        return new Random().Next(1, 1000); // ID fictício
    }
}

// ──────────────────────────────────────────────
// Exceções de domínio
// ──────────────────────────────────────────────

public class ProdutoNaoEncontradoException : Exception
{
    public ProdutoNaoEncontradoException(string message) : base(message) { }
}

public class EstoqueInsuficienteException : Exception
{
    public EstoqueInsuficienteException(string message) : base(message) { }
}

// ──────────────────────────────────────────────
// Testes
// ──────────────────────────────────────────────

public class PedidoServiceTests
{
    // ── Teste 1: fluxo feliz ──────────────────
    [Fact]
    public void CriarPedido_DeveRetornarIdDoPedido_QuandoDadosValidos()
    {
        // Arrange
        var mockProdutoRepo   = new Mock<IProdutoRepository>();
        var mockEstoqueService = new Mock<IEstoqueService>();

        var produtoExistente = new Produto
            { Id = 1, Nome = "Notebook", Preco = 2500m, Estoque = 10 };

        mockProdutoRepo
            .Setup(repo => repo.ObterPorId(1))
            .Returns(produtoExistente);

        var pedidoService = new PedidoService(
            mockProdutoRepo.Object, mockEstoqueService.Object);

        // Act
        int pedidoId = pedidoService.CriarPedido(1, 2);

        // Assert
        pedidoId.Should().BeGreaterThan(0);

        mockEstoqueService.Verify(
            s => s.DebitarEstoque(1, 2), Times.Once);

        mockProdutoRepo.Verify(
            r => r.ObterPorId(1), Times.Once);
    }

    // ── Teste 2: quantidade zero ──────────────
    [Fact]
    public void CriarPedido_DeveLancarArgumentException_QuandoQuantidadeForZero()
    {
        // Arrange
        var mockProdutoRepo    = new Mock<IProdutoRepository>();
        var mockEstoqueService = new Mock<IEstoqueService>();

        var pedidoService = new PedidoService(
            mockProdutoRepo.Object, mockEstoqueService.Object);

        // Act
        Action act = () => pedidoService.CriarPedido(1, 0);

        // Assert
        act.Should()
           .Throw<ArgumentException>()
           .WithMessage("A quantidade deve ser maior que zero.");

        // Garante que o estoque NÃO foi debitado
        mockEstoqueService.Verify(
            s => s.DebitarEstoque(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    // ── Teste 3: produto inexistente ──────────
    [Fact]
    public void CriarPedido_DeveLancarProdutoNaoEncontradoException_QuandoProdutoInexistente()
    {
        // Arrange
        var mockProdutoRepo    = new Mock<IProdutoRepository>();
        var mockEstoqueService = new Mock<IEstoqueService>();

        mockProdutoRepo
            .Setup(repo => repo.ObterPorId(It.IsAny<int>()))
            .Returns((Produto)null);

        var pedidoService = new PedidoService(
            mockProdutoRepo.Object, mockEstoqueService.Object);

        // Act
        Action act = () => pedidoService.CriarPedido(99, 1);

        // Assert
        act.Should()
           .Throw<ProdutoNaoEncontradoException>()
           .WithMessage("Produto com ID 99 não encontrado.");

        mockEstoqueService.Verify(
            s => s.DebitarEstoque(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    // ── Teste 4: estoque insuficiente ─────────
    [Fact]
    public void CriarPedido_DeveLancarEstoqueInsuficienteException_QuandoEstoqueForInsuficiente()
    {
        // Arrange
        var mockProdutoRepo    = new Mock<IProdutoRepository>();
        var mockEstoqueService = new Mock<IEstoqueService>();

        var produtoComPoucoEstoque = new Produto
            { Id = 1, Nome = "Teclado", Preco = 100m, Estoque = 1 };

        mockProdutoRepo
            .Setup(repo => repo.ObterPorId(1))
            .Returns(produtoComPoucoEstoque);

        var pedidoService = new PedidoService(
            mockProdutoRepo.Object, mockEstoqueService.Object);

        // Act
        Action act = () => pedidoService.CriarPedido(1, 5); // pede 5, tem 1

        // Assert
        act.Should()
           .Throw<EstoqueInsuficienteException>()
           .WithMessage("Estoque insuficiente para o produto Teclado.");

        mockEstoqueService.Verify(
            s => s.DebitarEstoque(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }
}