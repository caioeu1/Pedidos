namespace Users.Domain.ValueObjects;

public sealed record Endereco(
    string Rua,
    string Numero,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep)
{
    public override string ToString() =>
        $"{Rua}, {Numero} - {Bairro}, {Cidade}/{Estado} - CEP: {Cep}";
}
