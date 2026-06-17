using TruckAndRoll.Api.Domain;
using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Repositories;

/// <summary>
/// PATRÓN: Repository (interfaces abstractas)
///
/// Definen el contrato de acceso a datos sin atarlo a una tecnología concreta.
/// Los servicios dependen de estas abstracciones, no de la implementación
/// in-memory. Esto es Dependency Inversion (la 'D' de SOLID): cambiar a una
/// base real sería agregar otra implementación sin tocar los servicios.
///
/// Repository ya aparecía en el diagrama de clases de TFU3 (PedidoRepository).
/// </summary>
public interface IProductoRepository
{
    IReadOnlyList<Producto> Listar();
    Producto? Obtener(string productoId);
}

public interface IPedidoRepository
{
    Pedido Guardar(Pedido pedido);
    Pedido? Obtener(int numero);
    IReadOnlyList<Pedido> Listar(EstadoPedido? estado = null);
    int ProximoNumero();
}
