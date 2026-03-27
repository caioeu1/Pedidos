namespace Orders.Domain.Exceptions;

public class PedidoInvalidoException : Exception
{
    public PedidoInvalidoException(string mensagem) : base(mensagem) { }
}
