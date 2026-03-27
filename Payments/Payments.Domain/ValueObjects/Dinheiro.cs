namespace Payments.Domain.ValueObjects;

public sealed record Dinheiro(decimal Valor, string Moeda = "BRL");
