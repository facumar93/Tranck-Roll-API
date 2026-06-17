using Microsoft.AspNetCore.Mvc;
using TruckAndRoll.Api.Dtos;
using TruckAndRoll.Api.Services;

namespace TruckAndRoll.Api.Controllers;

/// <summary>
/// Endpoint del menú. Soporta la pantalla "Crear Pedido" del prototipo de UT4.
/// </summary>
[ApiController]
[Route("menu")]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>Devuelve los productos disponibles del food truck.</summary>
    [HttpGet]
    public ActionResult<List<ProductoOut>> ListarMenu()
        => Ok(_menuService.ListarMenu());
}
