using Microsoft.AspNetCore.Mvc;
using Orders.Application.DTOs;
using Orders.Application.UseCases.CreateOrder;
using Orders.Application.UseCases.GetOrder;
using Orders.Application.UseCases.UpdateOrderStatus;
using Orders.Domain.Enums;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly CreateOrderHandler _createHandler;
    private readonly GetOrderHandler _getHandler;
    private readonly UpdateOrderStatusHandler _updateHandler;

    public PedidosController(
        CreateOrderHandler createHandler,
        GetOrderHandler getHandler,
        UpdateOrderStatusHandler updateHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _updateHandler = updateHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoRequest request, CancellationToken ct)
    {
        var command = new CreateOrderCommand(request.UsuarioId, request.Itens);
        var resultado = await _createHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(ObterPedido), new { id = resultado.Id }, resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPedido(Guid id, CancellationToken ct)
    {
        var resultado = await _getHandler.HandleAsync(new GetOrderQuery(id), ct);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelarPedido(Guid id, CancellationToken ct)
    {
        await _updateHandler.HandleAsync(new UpdateOrderStatusCommand(id, OrderStatus.Cancelled), ct);
        return NoContent();
    }
}
