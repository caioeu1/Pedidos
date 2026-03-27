using Users.Domain.ValueObjects;

namespace Users.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Credenciais Credenciais { get; private set; } = null!;
    public Endereco? Endereco { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public bool Ativo { get; private set; }

    private Usuario() { }

    public static Usuario Criar(string nome, string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Credenciais = Credenciais.Criar(email, senhaHash),
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };
    }

    public void AtualizarEndereco(Endereco endereco) => Endereco = endereco;

    public void Desativar() => Ativo = false;
}
