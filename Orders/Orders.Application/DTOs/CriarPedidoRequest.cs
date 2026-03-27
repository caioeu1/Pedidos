namespace Orders.Application.DTOs;

public record CriarPedidoRequest(
    Guid UsuarioId,
    List<ItemPedidoRequest> Itens);

public record ItemPedidoRequest(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario);
