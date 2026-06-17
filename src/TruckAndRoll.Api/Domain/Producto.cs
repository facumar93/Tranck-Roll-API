namespace TruckAndRoll.Api.Domain;

/// <summary>
/// Item del menú del food truck. Entidad del modelo de dominio (TFU3).
/// </summary>
public class Producto
{
    public string Id { get; }
    public string Nombre { get; }
    public decimal Precio { get; }
    public string Descripcion { get; }

    public Producto(string id, string nombre, decimal precio, string descripcion = "")
    {
        Id = id;
        Nombre = nombre;
        Precio = precio;
        Descripcion = descripcion;
    }
}
