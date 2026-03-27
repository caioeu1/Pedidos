using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Moq;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Orders.Application.Interfaces;
using Orders.Domain.ValueObjects;

public class PedidosApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PedidosApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Monta um request válido com 1 item para reaproveitar nos testes.
    /// </summary>
    private static CriarPedidoRequest BuildRequestValido() =>
        new(
            UsuarioId: Guid.NewGuid(),
            Itens: new List<ItemPedidoRequest>
            {
                new(
                    ProdutoId:      Guid.NewGuid(),
                    NomeProduto:    "Notebook Gamer",
                    Quantidade:     2,
                    PrecoUnitario:  3500m
                )
            }
        );

    /// <summary>
    /// Cria um HttpClient com IPedidoRepository e IUnitOfWork mockados,
    /// substituindo o banco SQLite real — ideal para testes isolados.
    /// </summary>
    private HttpClient BuildClientComMocks(
        Mock<IPedidoRepository> mockRepo,
        Mock<IUnitOfWork> mockUow)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Substitui IPedidoRepository pelo mock
                var descRepo = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IPedidoRepository));
                if (descRepo != null) services.Remove(descRepo);
                services.AddScoped<IPedidoRepository>(_ => mockRepo.Object);

                // Substitui IUnitOfWork pelo mock
                var descUow = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IUnitOfWork));
                if (descUow != null) services.Remove(descUow);
                services.AddScoped<IUnitOfWork>(_ => mockUow.Object);
            });
        });

        return factory.CreateClient();
    }

    // ── Testes ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_CriarPedido_DeveRetornarCreated_QuandoDadosValidos()
    {
        // Arrange
        var mockRepo = new Mock<IPedidoRepository>();
        var mockUow  = new Mock<IUnitOfWork>();

        mockRepo
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var client  = BuildClientComMocks(mockRepo, mockUow);
        var request = BuildRequestValido();

        // Act
        var response = await client.PostAsJsonAsync("/api/pedidos", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<PedidoResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
        body.ValorTotal.Should().BeGreaterThan(0);

        mockRepo.Verify(
            r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()),
            Times.Once);

        mockUow.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Post_CriarPedido_DeveRetornarBadRequest_QuandoListaDeItensVazia()
    {
        // Arrange
        var mockRepo = new Mock<IPedidoRepository>();
        var mockUow  = new Mock<IUnitOfWork>();

        var client = BuildClientComMocks(mockRepo, mockUow);

        var requestSemItens = new CriarPedidoRequest(
            UsuarioId: Guid.NewGuid(),
            Itens: new List<ItemPedidoRequest>() // lista vazia
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/pedidos", requestSemItens);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        mockRepo.Verify(
            r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Get_ObterPedido_DeveRetornarOk_QuandoPedidoExiste()
    {
        // Arrange
        var mockRepo = new Mock<IPedidoRepository>();
        var mockUow  = new Mock<IUnitOfWork>();

        // Primeiro cria um pedido para ter um ID válido
        mockRepo
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var client  = BuildClientComMocks(mockRepo, mockUow);
        var request = BuildRequestValido();

        var postResponse = await client.PostAsJsonAsync("/api/pedidos", request);
        var pedidoCriado = await postResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        // Configura o mock para retornar o pedido ao buscar por ID
        mockRepo
            .Setup(r => r.ObterPorIdAsync(pedidoCriado!.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Pedido.Criar(request.UsuarioId));

        // Act
        var getResponse = await client.GetAsync($"/api/pedidos/{pedidoCriado!.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ObterPedido_DeveRetornarNotFound_QuandoPedidoNaoExiste()
    {
        // Arrange
        var mockRepo = new Mock<IPedidoRepository>();
        var mockUow  = new Mock<IUnitOfWork>();

        mockRepo
            .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido?)null);

        var client = BuildClientComMocks(mockRepo, mockUow);

        // Act
        var response = await client.GetAsync($"/api/pedidos/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Patch_CancelarPedido_DeveRetornarNoContent_QuandoPedidoExiste()
    {
        // Arrange
        var mockRepo = new Mock<IPedidoRepository>();
        var mockUow  = new Mock<IUnitOfWork>();

        // Cria o pedido com um item para poder Confirmar()
        var pedido = Pedido.Criar(Guid.NewGuid());
        var item = ItemPedido.Criar(Guid.NewGuid(), "Notebook", 1, 2500m);
        pedido.AdicionarItem(item);
        pedido.Confirmar();

        mockRepo
            .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        mockRepo
            .Setup(r => r.AtualizarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUow
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var client = BuildClientComMocks(mockRepo, mockUow);

        // Act
        var response = await client.PatchAsync($"/api/pedidos/{pedido.Id}/cancelar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    } 
