using Microsoft.AspNetCore.Mvc;
using TruckAndRoll.Api.Domain.Enums;
using TruckAndRoll.Api.Dtos;
using TruckAndRoll.Api.Patterns;
using TruckAndRoll.Api.Services;

namespace TruckAndRoll.Api.Controllers;

/// <summary>
/// Fachada de compatibilidad para el prototipo Figma Make.
///
/// La API canonica mantiene nombres y estados del dominio en espanol. Esta
/// capa traduce al contrato con campos en ingles que usa el frontend, sin
/// cambiar las entidades del modelo.
/// </summary>
[ApiController]
[Route("frontend")]
[Produces("application/json")]
public class FrontendController : ControllerBase
{
    private readonly MenuService _menuService;
    private readonly PedidoService _pedidoService;

    public FrontendController(MenuService menuService, PedidoService pedidoService)
    {
        _menuService = menuService;
        _pedidoService = pedidoService;
    }

    /// <summary>Productos con la forma esperada por la pantalla "Crear Pedido".</summary>
    [HttpGet("menu")]
    public ActionResult<List<ProductoFrontendOut>> ListarMenu()
        => Ok(_menuService.ListarMenu().Select(FrontendMapper.AProductoFrontend).ToList());

    /// <summary>Crea un pedido aceptando la forma de datos del prototipo.</summary>
    [HttpPost("pedidos")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PedidoFrontendOut> CrearPedido([FromBody] PedidoFrontendCreateIn datos)
    {
        try
        {
            var canonico = FrontendMapper.APedidoCanonico(datos);
            var creado = _pedidoService.CrearPedido(canonico);
            return CreatedAtAction(
                nameof(ObtenerPedido),
                new { numero = creado.Numero },
                FrontendMapper.APedidoFrontend(creado));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { detail = e.Message });
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

    /// <summary>Pedidos con la forma esperada por la pantalla "Estado de Pedidos".</summary>
    [HttpGet("pedidos")]
    public ActionResult<List<PedidoFrontendOut>> ListarPedidos([FromQuery] EstadoPedido? estado)
        => Ok(_pedidoService.ListarPedidos(estado)
            .Select(FrontendMapper.APedidoFrontend)
            .ToList());

    /// <summary>Pedido puntual con la forma esperada por el prototipo.</summary>
    [HttpGet("pedidos/{numero:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PedidoFrontendOut> ObtenerPedido(int numero)
    {
        try
        {
            return Ok(FrontendMapper.APedidoFrontend(_pedidoService.ObtenerPedido(numero)));
        }
        catch (PedidoNoEncontradoException e)
        {
            return NotFound(new { detail = e.Message });
        }
    }
}
