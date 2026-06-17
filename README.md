# Truck & Roll — API de apoyo operativo (UT5 · TFU)

API REST DEMO para el food truck **Truck & Roll**, desarrollada en **C# / .NET 8 (ASP.NET Core Web API)**.

El objetivo de esta entrega no es una API completa, sino hacer **"caminar" dos casos de uso del producto** de punta a punta, los mismos dos que ya están prototipados en Figma (UT4):

- **"Crear Pedido"** (pantalla del cajero): ver el menú, seleccionar productos y registrar el pedido.
- **"Estado de Pedidos"**: ver los pedidos por estado y avanzarlos en su ciclo de vida.

El detalle de qué patrones se aplicaron y qué se modificó respecto de las unidades anteriores está en **[DISENO.md](DISENO.md)**. La forma de datos del frontend está en **[CONTEXTO_FRONTEND.md](CONTEXTO_FRONTEND.md)**.

---

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior

Verificá la instalación con:

```bash
dotnet --version
```

## Ejecutar la API

```bash
cd src/TruckAndRoll.Api
dotnet restore      # descarga dependencias (Swashbuckle)
dotnet run
```

La API queda en `http://localhost:5080`.

## Probar los endpoints

Hay dos formas:

1. **Swagger UI (recomendado, no requiere nada extra):**
   abrir `http://localhost:5080/swagger` en el navegador. Permite ejecutar cada
   endpoint desde el navegador y ver los esquemas de entrada/salida. Esto es,
   además, la **documentación del contrato de la API** que un equipo de
   frontend externo (outsourcing) usaría para consumirla.

2. **REST Client de VS Code:** abrir [`requests.http`](requests.http) y hacer
   clic en *"Send Request"* sobre cada bloque. Incluye el flujo completo
   (ver menú → crear pedido → avanzar estados) y dos casos de error para
   mostrar las validaciones.

---

## Endpoints

| Método | Ruta | Pantalla (UT4) | Descripción |
|--------|------|----------------|-------------|
| `GET`  | `/menu` | "Crear Pedido" (soporte) | Lista los productos del food truck |
| `POST` | `/pedidos` | "Crear Pedido" | Crea un pedido a partir de productos seleccionados |
| `GET`  | `/pedidos` | "Estado de Pedidos" | Lista pedidos; admite `?estado=EN_PREPARACION` |
| `GET`  | `/pedidos/{numero}` | "Estado de Pedidos" | Devuelve un pedido puntual |
| `PATCH`| `/pedidos/{numero}/estado` | "Estado de Pedidos" | Avanza el estado del pedido |

### Endpoints compatibles con el prototipo Figma Make

La API mantiene endpoints canónicos en español y además expone una fachada para
el frontend, con los nombres de campos que usa el prototipo:

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET`  | `/frontend/menu` | Productos con `id`, `name`, `price`, `category`, `image`, `color` |
| `POST` | `/frontend/pedidos` | Crea pedidos aceptando `customerName`, `nickname`, `items[].product.id` o `items[].productId` |
| `GET`  | `/frontend/pedidos` | Lista pedidos con `number`, `customerName`, `nickname`, `items`, `status`, `estimatedTime` |
| `GET`  | `/frontend/pedidos/{numero}` | Devuelve un pedido en formato del frontend |

### Ciclo de vida del estado de un pedido

```
REGISTRADO ──> EN_PREPARACION ──> LISTO ──> ENTREGADO
     │                  │
     └──> CANCELADO <────┘
```

Las transiciones fuera de este ciclo (por ejemplo saltar de `REGISTRADO` a
`ENTREGADO`) se rechazan con **HTTP 409**.

---

## Estructura del proyecto

```
src/TruckAndRoll.Api/
├── Program.cs            # Composition root: DI, Swagger, pipeline HTTP
├── Domain/               # Dominio: entidades y enums  [de TFU3]
│   ├── Producto.cs
│   ├── ItemPedido.cs
│   ├── Pedido.cs
│   └── Enums/Estados.cs
├── Dtos/                 # DTOs (records): validación + serialización
├── Repositories/         # Patrón Repository: interfaces + in-memory
├── Patterns/             # Builder (crear pedido) y máquina de estados (State)
├── Services/             # Lógica de aplicación + mapper para Figma Make
└── Controllers/          # Endpoints HTTP (menú y pedidos)
```

## Notas sobre la persistencia

Los datos se guardan **en memoria** (se reinician al reiniciar el servidor).
Es una decisión deliberada para la versión DEMO. Gracias al patrón Repository,
cambiar a una base real implicaría agregar una nueva implementación de las
interfaces `IProductoRepository` / `IPedidoRepository` y registrarla en
`Program.cs`, sin tocar los servicios.
