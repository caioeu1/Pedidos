namespace Users.Domain.ValueObjects;

public sealed record Credenciais(string Email, string SenhaHash)
{
    public static Credenciais Criar(string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("E-mail inválido.", nameof(email));
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Senha não pode ser vazia.", nameof(senhaHash));
        return new Credenciais(email.ToLowerInvariant(), senhaHash);
    }
}
