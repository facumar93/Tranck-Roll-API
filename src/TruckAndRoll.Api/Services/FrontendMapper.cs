using TruckAndRoll.Api.Domain.Enums;
using TruckAndRoll.Api.Dtos;

namespace TruckAndRoll.Api.Services;

/// <summary>
/// Capa de traduccion entre el contrato canonico de la API y la forma exacta
/// que espera el prototipo Figma Make. Mantiene al dominio libre de datos
/// cosmeticos de UI.
/// </summary>
public static class FrontendMapper
{
    private static readonly Dictionary<string, (string Category, string Image, string Color)> ProductoUi = new()
    {
        ["1"] = ("Combos", "combo", "#16A34A"),
        ["2"] = ("Hamburguesas", "burger", "#F97316"),
        ["3"] = ("Papas", "fries", "#EAB308"),
        ["4"] = ("Bebidas", "drink", "#0EA5E9"),
        ["5"] = ("Papas", "wrap", "#7C3AED"),
        ["6"] = ("Bebidas", "lemonade", "#DB2777")
    };

    private static readonly Dictionary<EstadoPedido, (string Status, int EstimatedTime, string Color, string Icon)> PedidoUi = new()
    {
        [EstadoPedido.REGISTRADO] = ("cola", 5, "#0B74DE", "hourglass"),
        [EstadoPedido.EN_PREPARACION] = ("preparacion", 3, "#F97316", "chef-hat"),
        [EstadoPedido.LISTO] = ("recibido", 1, "#7C3AED", "clipboard-check"),
        [EstadoPedido.ENTREGADO] = ("recibido", 0, "#16A34A", "check"),
        [EstadoPedido.CANCELADO] = ("cancelado", 0, "#DC2626", "x")
    };

    public static ProductoFrontendOut AProductoFrontend(ProductoOut producto)
    {
        if (!ProductoUi.TryGetValue(producto.Id, out var ui))
            ui = ("Otros", "product", "#64748B");

        return new ProductoFrontendOut(
            Id: producto.Id,
            Name: producto.Nombre,
            Price: producto.Precio,
            Category: ui.Category,
            Image: ui.Image,
            Color: ui.Color);
    }

    public static PedidoFrontendOut APedidoFrontend(PedidoOut pedido)
    {
        if (!PedidoUi.TryGetValue(pedido.Estado, out var ui))
            ui = ("cola", 5, "#64748B", "receipt");

        var (nombre, apodo) = SepararNombreYApodo(pedido.NombreCliente);

        return new PedidoFrontendOut(
            Id: pedido.Numero,
            Number: pedido.Numero,
            CustomerName: nombre,
            Nickname: apodo,
            Items: pedido.Items.Select(FormatearItem).ToList(),
            Status: ui.Status,
            EstimatedTime: ui.EstimatedTime,
            Color: ui.Color,
            Icon: ui.Icon);
    }

    public static PedidoCreateIn APedidoCanonico(PedidoFrontendCreateIn datos)
    {
        var items = datos.Items.Select(item =>
        {
            var productoId = item.ProductId ?? item.Product?.Id;
            if (string.IsNullOrWhiteSpace(productoId))
                throw new ArgumentException("Cada item debe indicar productId o product.id.");

            return new ItemPedidoIn
            {
                ProductoId = productoId,
                Cantidad = item.Quantity
            };
        }).ToList();

        return new PedidoCreateIn
        {
            Items = items,
            NombreCliente = CombinarNombreYApodo(datos.CustomerName, datos.Nickname),
            Nota = datos.Note
        };
    }

    private static string FormatearItem(ItemPedidoOut item)
        => item.Cantidad == 1
            ? item.Nombre
            : $"{item.Cantidad}x {item.Nombre}";

    private static (string? Nombre, string? Apodo) SepararNombreYApodo(string? nombreCliente)
    {
        if (string.IsNullOrWhiteSpace(nombreCliente))
            return (null, null);

        var partes = nombreCliente.Split('/', 2, StringSplitOptions.TrimEntries);
        return partes.Length == 2
            ? (partes[0], partes[1])
            : (nombreCliente, null);
    }

    private static string? CombinarNombreYApodo(string? nombre, string? apodo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return string.IsNullOrWhiteSpace(apodo) ? null : apodo.Trim();

        return string.IsNullOrWhiteSpace(apodo)
            ? nombre.Trim()
            : $"{nombre.Trim()} / {apodo.Trim()}";
    }
}
