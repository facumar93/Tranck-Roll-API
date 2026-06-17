using TruckAndRoll.Api.Domain;

namespace TruckAndRoll.Api.Repositories;

/// <summary>
/// Catálogo de productos en memoria.
///
/// Se siembra con los items visibles en la pantalla "Crear Pedido" del
/// prototipo de UT4 (ver CONTEXTO_FRONTEND.md). Los precios y nombres
/// coinciden con los del frontend.
/// </summary>
public class InMemoryProductoRepository : IProductoRepository
{
    private readonly Dictionary<string, Producto> _productos;

    public InMemoryProductoRepository()
    {
        var menu = new List<Producto>
        {
            new("1", "Combo 1", 290m, "Hamburguesa + papas + bebida"),
            new("2", "Hamburguesa con queso", 220m, "Clásica con queso cheddar"),
            new("3", "Papas grandes", 150m, "Porción grande de papas fritas"),
            new("4", "Coca Light", 110m, "Lata 354ml"),
            new("5", "Wrap veggie", 210m, "Wrap vegetariano"),
            new("6", "Limonada", 120m, "Limonada natural"),
        };
        _productos = menu.ToDictionary(p => p.Id);
    }

    public IReadOnlyList<Producto> Listar() => _productos.Values.ToList();

    public Producto? Obtener(string productoId)
        => _productos.TryGetValue(productoId, out var p) ? p : null;
}
