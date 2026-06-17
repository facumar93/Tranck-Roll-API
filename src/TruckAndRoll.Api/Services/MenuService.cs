using TruckAndRoll.Api.Dtos;
using TruckAndRoll.Api.Repositories;

namespace TruckAndRoll.Api.Services;

/// <summary>Se referenció un producto que no existe en el menú.</summary>
public class ProductoNoEncontradoException : Exception
{
    public ProductoNoEncontradoException(string message) : base(message) { }
}

/// <summary>Se referenció un pedido que no existe.</summary>
public class PedidoNoEncontradoException : Exception
{
    public PedidoNoEncontradoException(string message) : base(message) { }
}

/// <summary>
/// Servicio de consulta del menú (soporte para la pantalla "Crear Pedido").
/// </summary>
public class MenuService
{
    private readonly IProductoRepository _productoRepo;

    public MenuService(IProductoRepository productoRepo)
    {
        _productoRepo = productoRepo;
    }

    public List<ProductoOut> ListarMenu()
        => _productoRepo.Listar()
            .Select(p => new ProductoOut(p.Id, p.Nombre, p.Precio, p.Descripcion))
            .ToList();
}
