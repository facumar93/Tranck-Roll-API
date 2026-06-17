using TruckAndRoll.Api.Domain;
using TruckAndRoll.Api.Domain.Enums;
using TruckAndRoll.Api.Dtos;
using TruckAndRoll.Api.Patterns;
using TruckAndRoll.Api.Repositories;

namespace TruckAndRoll.Api.Services;

/// <summary>
/// Servicio de pedidos. Orquesta el caso de uso sin conocer detalles de HTTP.
/// Depende de las ABSTRACCIONES de repositorio (no de la implementación
/// in-memory), cumpliendo Dependency Inversion. Usa el Builder para construir
/// el pedido (pantalla "Crear Pedido") y la máquina de estados para avanzarlo
/// (pantalla "Estado de Pedidos"). Mapea entidades de dominio a DTOs de salida.
/// </summary>
public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepo;
    private readonly IProductoRepository _productoRepo;

    public PedidoService(IPedidoRepository pedidoRepo, IProductoRepository productoRepo)
    {
        _pedidoRepo = pedidoRepo;
        _productoRepo = productoRepo;
    }

    // --- Pantalla "Crear Pedido": registrar un pedido ----------------------
    public PedidoOut CrearPedido(PedidoCreateIn datos)
    {
        var numero = _pedidoRepo.ProximoNumero();
        var builder = new PedidoBuilder(numero);

        foreach (var item in datos.Items)
        {
            var producto = _productoRepo.Obtener(item.ProductoId);
            if (producto is null)
                throw new ProductoNoEncontradoException(
                    $"El producto '{item.ProductoId}' no existe en el menú.");
            builder.AgregarItem(producto, item.Cantidad);
        }

        builder.ConNombreCliente(datos.NombreCliente).ConNota(datos.Nota);

        var pedido = builder.Build(); // puede lanzar PedidoVacioException
        _pedidoRepo.Guardar(pedido);
        return ADto(pedido);
    }

    // --- Pantalla "Estado de Pedidos": listar ------------------------------
    public List<PedidoOut> ListarPedidos(EstadoPedido? estado = null)
        => _pedidoRepo.Listar(estado).Select(ADto).ToList();

    public PedidoOut ObtenerPedido(int numero)
    {
        var pedido = _pedidoRepo.Obtener(numero)
            ?? throw new PedidoNoEncontradoException($"No existe el pedido número {numero}.");
        return ADto(pedido);
    }

    // --- Pantalla "Estado de Pedidos": avanzar estado ----------------------
    public PedidoOut CambiarEstado(int numero, EstadoPedido nuevoEstado)
    {
        var pedido = _pedidoRepo.Obtener(numero)
            ?? throw new PedidoNoEncontradoException($"No existe el pedido número {numero}.");

        // La máquina de estados valida la transición (puede lanzar
        // TransicionInvalidaException, traducida a HTTP 409 en el controller).
        MaquinaEstadosPedido.Validar(pedido.Estado, nuevoEstado);
        pedido.Estado = nuevoEstado;
        _pedidoRepo.Guardar(pedido);
        return ADto(pedido);
    }

    // --- Mapeo dominio -> DTO ----------------------------------------------
    private static PedidoOut ADto(Pedido pedido)
    {
        var items = pedido.Items
            .Select(i => new ItemPedidoOut(
                i.Producto.Id,
                i.Producto.Nombre,
                i.Producto.Precio,
                i.Cantidad,
                i.Subtotal()))
            .ToList();

        return new PedidoOut(
            Numero: pedido.Numero,
            Estado: pedido.Estado,
            NombreCliente: pedido.NombreCliente,
            Nota: pedido.Nota,
            Items: items,
            Total: pedido.CalcularTotal(),
            FechaHora: pedido.FechaHora.ToString("s"),
            EstadosSiguientes: MaquinaEstadosPedido.EstadosSiguientes(pedido.Estado));
    }
}
