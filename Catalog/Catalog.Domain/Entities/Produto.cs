using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public Dinheiro Preco { get; private set; } = null!;
    public int QuantidadeEstoque { get; private set; }
    public bool Ativo { get; private set; }

    private Produto() { }

    public static Produto Criar(string nome, string descricao, decimal preco, int estoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(preco));
        if (estoque < 0)
            throw new ArgumentException("Estoque não pode ser negativo.", nameof(estoque));

        return new Produto
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Descricao = descricao,
            Preco = new Dinheiro(preco),
            QuantidadeEstoque = estoque,
            Ativo = true
        };
    }

    public void AtualizarEstoque(int novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new InvalidOperationException("Estoque não pode ser negativo.");
        QuantidadeEstoque = novaQuantidade;
    }

    public bool TemEstoque(int quantidade) => QuantidadeEstoque >= quantidade;
}
