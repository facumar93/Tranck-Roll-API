using System.ComponentModel.DataAnnotations;
using TruckAndRoll.Api.Domain.Enums;

namespace TruckAndRoll.Api.Dtos;

// ---------------------------------------------------------------------------
// DTOs (records) de la API.
//
// Separan la capa de presentación/validación del modelo de dominio (clases en
// Domain/). ASP.NET Core los usa para validar el cuerpo de las peticiones y
// para serializar respuestas, y para autogenerar la documentación OpenAPI
// (Swagger). Esta separación es parte de aplicar SRP / MVC: el dominio no
// conoce nada de HTTP ni de validación de entrada.
// ---------------------------------------------------------------------------

/// <summary>Salida: un producto del menú.</summary>
public record ProductoOut(string Id, string Nombre, decimal Precio, string Descripcion);

/// <summary>Entrada: una línea del pedido al crearlo (pantalla "Crear Pedido").</summary>
public record ItemPedidoIn
{
    [Required]
    public string ProductoId { get; init; } = default!;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; init; } = 1;
}

/// <summary>Entrada: crear un pedido.</summary>
public record PedidoCreateIn
{
    [Required]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un producto.")]
    public List<ItemPedidoIn> Items { get; init; } = new();

    public string? NombreCliente { get; init; }
    public string? Nota { get; init; }
}

/// <summary>Entrada: avanzar el estado de un pedido (pantalla "Estado de Pedidos").</summary>
public record EstadoUpdateIn
{
    [Required]
    public EstadoPedido Estado { get; init; }
}

/// <summary>Salida: una línea del pedido.</summary>
public record ItemPedidoOut(
    string ProductoId,
    string Nombre,
    decimal PrecioUnitario,
    int Cantidad,
    decimal Subtotal);

/// <summary>Salida: un pedido completo.</summary>
public record PedidoOut(
    int Numero,
    EstadoPedido Estado,
    string? NombreCliente,
    string? Nota,
    List<ItemPedidoOut> Items,
    decimal Total,
    string FechaHora,
    List<EstadoPedido> EstadosSiguientes);

/// <summary>
/// Salida compatible con el prototipo Figma Make: nombres de campos y metadatos
/// de UI esperados por la pantalla "Crear Pedido".
/// </summary>
public record ProductoFrontendOut(
    string Id,
    string Name,
    decimal Price,
    string Category,
    string Image,
    string Color);

/// <summary>Referencia reducida al producto que puede venir desde el front.</summary>
public record ProductoFrontendRef(string? Id);

/// <summary>
/// Entrada compatible con el prototipo: acepta productId directo o un objeto
/// product con id, como usa el tipo OrderItem del front.
/// </summary>
public record ItemPedidoFrontendIn
{
    public string? ProductId { get; init; }
    public ProductoFrontendRef? Product { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Quantity { get; init; } = 1;
}

/// <summary>Entrada compatible con la pantalla "Crear Pedido" del prototipo.</summary>
public record PedidoFrontendCreateIn
{
    [Required]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un producto.")]
    public List<ItemPedidoFrontendIn> Items { get; init; } = new();

    public string? CustomerName { get; init; }
    public string? Nickname { get; init; }
    public string? Note { get; init; }
}

/// <summary>Salida compatible con la pantalla "Estado de Pedidos".</summary>
public record PedidoFrontendOut(
    int Id,
    int Number,
    string? CustomerName,
    string? Nickname,
    List<string> Items,
    string Status,
    int EstimatedTime,
    string Color,
    string Icon);
