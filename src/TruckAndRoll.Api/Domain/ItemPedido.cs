namespace TruckAndRoll.Api.Domain;

/// <summary>
/// Una línea dentro de un pedido: un producto y una cantidad.
/// Método Subtotal() tomado del modelo de TFU3.
/// </summary>
public class ItemPedido
{
    public Producto Producto { get; }
    public int Cantidad { get; set; }

    public ItemPedido(Producto producto, int cantidad = 1)
    {
        Producto = producto;
        Cantidad = cantidad;
    }

    /// <summary>Subtotal de la línea (precio * cantidad).</summary>
    public decimal Subtotal() => Math.Round(Producto.Precio * Cantidad, 2);
}
