namespace Orders.Domain.ValueObjects;

public class ItemPedido
{
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Total => Quantidade * PrecoUnitario;

    private ItemPedido() { }

    public static ItemPedido Criar(Guid produtoId, string nomeProduto, int quantidade, decimal preco)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (preco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero.", nameof(preco));

        return new ItemPedido
        {
            ProdutoId = produtoId,
            NomeProduto = nomeProduto,
            Quantidade = quantidade,
            PrecoUnitario = preco
        };
    }
}
