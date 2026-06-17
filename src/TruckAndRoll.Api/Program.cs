using System.Text.Json.Serialization;
using TruckAndRoll.Api.Repositories;
using TruckAndRoll.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers + serialización de enums como texto -----------------------
// Los enums (EstadoPedido) se serializan/deserializan como string
// ("EN_PREPARACION") en vez de número, para que la API sea legible y el
// frontend pueda enviarlos por nombre.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- Swagger / OpenAPI -----------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Truck & Roll - API de apoyo operativo",
        Version = "v1",
        Description =
            "API DEMO (UT5 - TFU). Hace 'caminar' dos casos de uso del producto:\n\n" +
            "- Pantalla 'Crear Pedido' (cajero)\n" +
            "- Pantalla 'Estado de Pedidos'\n\n" +
            "Patrones aplicados: Repository, Builder y máquina de estados (State)."
    });
});

// --- Composition Root (inyección de dependencias) --------------------------
// Aquí se eligen las implementaciones concretas (in-memory) y se cablean con
// los servicios. Cambiar de in-memory a otra persistencia sería tocar SOLO
// estas líneas. Se registran como Singleton para que los datos persistan
// durante toda la vida del proceso.
builder.Services.AddSingleton<IProductoRepository, InMemoryProductoRepository>();
builder.Services.AddSingleton<IPedidoRepository, InMemoryPedidoRepository>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<PedidoService>();

var app = builder.Build();

// --- Pipeline HTTP ---------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Truck & Roll API v1");
    c.RoutePrefix = "swagger"; // Swagger UI en /swagger
});

app.MapControllers();

// Endpoint de salud simple en la raíz.
app.MapGet("/", () => Results.Ok(new
{
    servicio = "Truck & Roll API",
    estado = "ok",
    documentacion = "/swagger"
}));

app.Run();
