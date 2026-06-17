using TruckAndRoll.Api.Domain;
using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Repositories;

/// <summary>
/// Pedidos en memoria, con numeración incremental.
///
/// Persistencia simple elegida a propósito para la versión DEMO. Gracias a la
/// abstracción IPedidoRepository, cambiar esto por una base real no afectaría
/// a los servicios.
/// </summary>
public class InMemoryPedidoRepository : IPedidoRepository
{
    private readonly Dictionary<int, Pedido> _pedidos = new();
    private int _contador = 0;
    private readonly object _lock = new();

    public int ProximoNumero()
    {
        lock (_lock)
        {
            return ++_contador;
        }
    }

    public Pedido Guardar(Pedido pedido)
    {
        _pedidos[pedido.Numero] = pedido;
        return pedido;
    }

    public Pedido? Obtener(int numero)
        => _pedidos.TryGetValue(numero, out var p) ? p : null;

    public IReadOnlyList<Pedido> Listar(EstadoPedido? estado = null)
    {
        var pedidos = _pedidos.Values.AsEnumerable();
        if (estado is not null)
            pedidos = pedidos.Where(p => p.Estado == estado);
        return pedidos.OrderBy(p => p.Numero).ToList();
    }
}
