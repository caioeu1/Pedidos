using Notifications.Domain.Enums;

namespace Notifications.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; private set; }
    public Guid DestinatarioId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Mensagem { get; private set; } = string.Empty;
    public CanalNotificacao Canal { get; private set; }
    public bool Enviada { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public DateTime? EnviadaEm { get; private set; }

    private Notificacao() { }

    public static Notificacao Criar(Guid destinatarioId, string titulo, string mensagem, CanalNotificacao canal)
    {
        return new Notificacao
        {
            Id = Guid.NewGuid(),
            DestinatarioId = destinatarioId,
            Titulo = titulo,
            Mensagem = mensagem,
            Canal = canal,
            Enviada = false,
            CriadaEm = DateTime.UtcNow
        };
    }

    public void MarcarComoEnviada()
    {
        Enviada = true;
        EnviadaEm = DateTime.UtcNow;
    }
}
