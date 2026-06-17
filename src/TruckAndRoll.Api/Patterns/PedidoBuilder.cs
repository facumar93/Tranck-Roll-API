using TruckAndRoll.Api.Domain;
using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Patterns;

/// <summary>Se intentó construir un pedido sin ninguna línea de producto.</summary>
public class PedidoVacioException : Exception
{
    public PedidoVacioException(string message) : base(message) { }
}

/// <summary>
/// PATRÓN: Builder
///
/// Construye un Pedido paso a paso: agrega líneas (ItemPedido) de a una y fija
/// datos opcionales (nombre del cliente, nota) antes de producir el objeto
/// final con Build().
///
/// Justificación (pantalla "Crear Pedido"): el cajero arma el pedido de forma
/// incremental — agrega productos uno por uno, ajusta cantidades, escribe una
/// nota opcional. El constructor de Pedido con muchos parámetros opcionales
/// sería el "telescoping constructor" que el TA1 de UT5 (ejercicio del Sandwich
/// y del TravelPlan) muestra como problema. Builder lo resuelve.
///
/// Relación con SOLID: Single Responsibility — la lógica de ensamblar el pedido
/// queda fuera de la entidad Pedido y fuera del controller.
/// </summary>
public class PedidoBuilder
{
    private readonly int _numero;
    private readonly List<ItemPedido> _items = new();
    private string? _nombreCliente;
    private string? _nota;

    public PedidoBuilder(int numero)
    {
        _numero = numero;
    }

    /// <summary>Agrega una línea. Si el producto ya está, suma la cantidad.</summary>
    public PedidoBuilder AgregarItem(Producto producto, int cantidad = 1)
    {
        if (cantidad < 1)
            throw new ArgumentException("La cantidad debe ser al menos 1.");

        var existente = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (existente is not null)
            existente.Cantidad += cantidad;
        else
            _items.Add(new ItemPedido(producto, cantidad));

        return this;
    }

    public PedidoBuilder ConNombreCliente(string? nombre)
    {
        _nombreCliente = nombre;
        return this;
    }

    public PedidoBuilder ConNota(string? nota)
    {
        _nota = nota;
        return this;
    }

    /// <summary>Produce el Pedido final. Falla si no tiene al menos una línea.</summary>
    public Pedido Build()
    {
        if (_items.Count == 0)
            throw new PedidoVacioException("Un pedido debe tener al menos un producto.");

        return new Pedido(
            numero: _numero,
            items: _items,
            estado: EstadoPedido.REGISTRADO,
            nombreCliente: _nombreCliente,
            nota: _nota);
    }
}
