namespace Catalog.Domain.ValueObjects;

public sealed record Dinheiro(decimal Valor, string Moeda = "BRL")
{
    public override string ToString() => $"{Moeda} {Valor:F2}";
}
