namespace Orders.Domain.ValueObjects;

public sealed record Dinheiro(decimal Valor, string Moeda = "BRL")
{
    public static Dinheiro Zero => new(0m);

    public Dinheiro Somar(Dinheiro outro)
    {
        if (Moeda != outro.Moeda)
            throw new InvalidOperationException("Não é possível somar valores em moedas diferentes.");
        return new Dinheiro(Valor + outro.Valor, Moeda);
    }

    public override string ToString() => $"{Moeda} {Valor:F2}";
}
