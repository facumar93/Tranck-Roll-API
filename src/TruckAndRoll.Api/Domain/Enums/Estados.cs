namespace TruckAndRoll.Api.Domain.Enums;

/// <summary>
/// Ciclo de vida de un pedido en el food truck.
/// Proviene del diagrama de clases de TFU3 (UT3).
/// </summary>
public enum EstadoPedido
{
    REGISTRADO,
    EN_PREPARACION,
    LISTO,
    ENTREGADO,
    CANCELADO
}

/// <summary>
/// Estado de un pago. Definido por consistencia con el modelo de TFU3.
/// El cobro no forma parte del núcleo demostrable de esta entrega.
/// </summary>
public enum EstadoPago
{
    PENDIENTE,
    APROBADO,
    RECHAZADO
}
