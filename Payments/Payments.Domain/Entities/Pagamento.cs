using Payments.Domain.Enums;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public class Pagamento
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Dinheiro Valor { get; private set; } = null!;
    public MetodoPagamento Metodo { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public string? CodigoTransacao { get; private set; }

    private Pagamento() { }

    public static Pagamento Criar(Guid pedidoId, decimal valor, MetodoPagamento metodo)
    {
        return new Pagamento
        {
            Id = Guid.NewGuid(),
            PedidoId = pedidoId,
            Valor = new Dinheiro(valor),
            Metodo = metodo,
            Status = PaymentStatus.Pending,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void Autorizar(string codigoTransacao)
    {
        Status = PaymentStatus.Authorized;
        CodigoTransacao = codigoTransacao;
    }

    public void Capturar()
    {
        if (Status != PaymentStatus.Authorized)
            throw new InvalidOperationException("Somente pagamentos autorizados podem ser capturados.");
        Status = PaymentStatus.Captured;
    }

    public void Falhar()
    {
        Status = PaymentStatus.Failed;
    }
}
