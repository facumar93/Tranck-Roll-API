using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Patterns;

/// <summary>
/// Se intentó pasar un pedido a un estado no permitido desde su estado actual.
/// </summary>
public class TransicionInvalidaException : Exception
{
    public TransicionInvalidaException(string message) : base(message) { }
}

/// <summary>
/// PATRÓN: Máquina de estados (State / State Machine ligero)
///
/// Centraliza en un solo lugar qué transiciones de EstadoPedido son válidas,
/// en lugar de repartir condicionales por los servicios. Sirve a la pantalla
/// "Estado de Pedidos" y evita estados imposibles (p. ej. saltar de REGISTRADO
/// directo a ENTREGADO).
///
/// Relación con SOLID: Open/Closed — agregar un estado o transición se hace
/// modificando el mapa Transiciones, sin tocar los servicios que lo usan.
/// </summary>
public static class MaquinaEstadosPedido
{
    private static readonly Dictionary<EstadoPedido, HashSet<EstadoPedido>> Transiciones = new()
    {
        [EstadoPedido.REGISTRADO] = new() { EstadoPedido.EN_PREPARACION, EstadoPedido.CANCELADO },
        [EstadoPedido.EN_PREPARACION] = new() { EstadoPedido.LISTO, EstadoPedido.CANCELADO },
        [EstadoPedido.LISTO] = new() { EstadoPedido.ENTREGADO },
        [EstadoPedido.ENTREGADO] = new(),   // estado final
        [EstadoPedido.CANCELADO] = new()    // estado final
    };

    public static bool TransicionPermitida(EstadoPedido actual, EstadoPedido destino)
        => Transiciones.TryGetValue(actual, out var destinos) && destinos.Contains(destino);

    /// <summary>Lanza TransicionInvalidaException si el cambio de estado no es válido.</summary>
    public static void Validar(EstadoPedido actual, EstadoPedido destino)
    {
        if (!TransicionPermitida(actual, destino))
        {
            var permitidos = EstadosSiguientes(actual);
            var texto = permitidos.Count > 0
                ? string.Join(", ", permitidos)
                : "ninguna (estado final)";
            throw new TransicionInvalidaException(
                $"No se puede pasar de {actual} a {destino}. " +
                $"Transiciones válidas desde {actual}: {texto}.");
        }
    }

    /// <summary>Estados a los que se puede avanzar desde el estado actual.</summary>
    public static List<EstadoPedido> EstadosSiguientes(EstadoPedido actual)
        => Transiciones.TryGetValue(actual, out var destinos)
            ? destinos.OrderBy(e => e.ToString()).ToList()
            : new List<EstadoPedido>();
}
