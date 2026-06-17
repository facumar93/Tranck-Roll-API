using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Domain;

/// <summary>
/// Pedido del food truck. Entidad central del modelo de dominio (TFU3).
///
/// El 'Numero' actúa como identificador de retiro simple (supuesto de UT2:
/// "un identificador simple, como número o nombre"). 'NombreCliente' es
/// opcional y replica el campo "Nombre o apodo" de la pantalla "Crear Pedido"
/// del prototipo de UT4.
/// </summary>
public class Pedido
{
    public int Numero { get; }
    public List<ItemPedido> Items { get; }
    public EstadoPedido Estado { get; set; }
    public string? NombreCliente { get; }
    public string? Nota { get; }
    public DateTime FechaHora { get; }

    public Pedido(
        int numero,
        List<ItemPedido> items,
        EstadoPedido estado = EstadoPedido.REGISTRADO,
        string? nombreCliente = null,
        string? nota = null)
    {
        Numero = numero;
        Items = items;
        Estado = estado;
        NombreCliente = nombreCliente;
        Nota = nota;
        FechaHora = DateTime.Now;
    }

    /// <summary>Total del pedido como suma de subtotales. Método del modelo TFU3.</summary>
    public decimal CalcularTotal() => Math.Round(Items.Sum(i => i.Subtotal()), 2);
}
