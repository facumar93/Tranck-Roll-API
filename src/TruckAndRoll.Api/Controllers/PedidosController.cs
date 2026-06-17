using Microsoft.AspNetCore.Mvc;
using TruckAndRoll.Api.Domain.Enums;
using TruckAndRoll.Api.Dtos;
using TruckAndRoll.Api.Patterns;
using TruckAndRoll.Api.Services;

namespace TruckAndRoll.Api.Controllers;

/// <summary>
/// Endpoints de pedidos. Cubre las dos pantallas del núcleo demostrable:
///
///   "Crear Pedido":
///     POST /pedidos
///
///   "Estado de Pedidos":
///     GET   /pedidos?estado=...
///     GET   /pedidos/{numero}
///     PATCH /pedidos/{numero}/estado
///
/// El controller solo traduce entre HTTP y la capa de servicio, y mapea las
/// excepciones de dominio a códigos HTTP. No contiene lógica de negocio.
/// </summary>
[ApiController]
[Route("pedidos")]
[Produces("application/json")]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidosController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    /// <summary>Crea un nuevo pedido (pantalla "Crear Pedido").</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PedidoOut> CrearPedido([FromBody] PedidoCreateIn datos)
    {
        try
        {
            var creado = _pedidoService.CrearPedido(datos);
            return CreatedAtAction(nameof(ObtenerPedido), new { numero = creado.Numero }, creado);
        }
        catch (ProductoNoEncontradoException e)
        {
            return NotFound(new { detail = e.Message });
        }
        catch (PedidoVacioException e)
        {
            return BadRequest(new { detail = e.Message });
        }
    }

    /// <summary>Lista pedidos, opcionalmente filtrados por estado (pantalla "Estado de Pedidos").</summary>
    [HttpGet]
    public ActionResult<List<PedidoOut>> ListarPedidos([FromQuery] EstadoPedido? estado)
        => Ok(_pedidoService.ListarPedidos(estado));

    /// <summary>Devuelve un pedido puntual.</summary>
    [HttpGet("{numero:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PedidoOut> ObtenerPedido(int numero)
    {
        try
        {
            return Ok(_pedidoService.ObtenerPedido(numero));
        }
        catch (PedidoNoEncontradoException e)
        {
            return NotFound(new { detail = e.Message });
        }
    }

    /// <summary>
    /// Avanza el estado del pedido respetando el ciclo de vida válido
    /// (REGISTRADO -> EN_PREPARACION -> LISTO -> ENTREGADO; o -> CANCELADO).
    /// </summary>
    [HttpPatch("{numero:int}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<PedidoOut> CambiarEstado(int numero, [FromBody] EstadoUpdateIn datos)
    {
        try
        {
            return Ok(_pedidoService.CambiarEstado(numero, datos.Estado));
        }
        catch (PedidoNoEncontradoException e)
        {
            return NotFound(new { detail = e.Message });
        }
        catch (TransicionInvalidaException e)
        {
            // 409 Conflict: el recurso existe pero el cambio pedido no es válido ahora.
            return Conflict(new { detail = e.Message });
        }
    }
}
