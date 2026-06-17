# CONTEXTO DEL FRONTEND (prototipo Figma Make) — para alinear la API

> Las dos pantallas del prototipo son "Crear Pedido" (operada por el cajero) y
> "Estado de Pedidos". Son los dos casos de uso elegidos para esta entrega por
> su valor para la solución que pide el negocio. Nos referimos a ellas por
> nombre y no por número de PUC, para evitar la ambigüedad de numeración que
> arrastran los documentos de UT2/UT4.

Este documento describe la forma de datos EXACTA que usa el prototipo de Figma
(componentes React `OrderCreation.tsx` y `OrderStatus.tsx`). La API debe poder
servir/recibir datos compatibles con esto a través de una capa de mapeo (los
DTOs traducen entre el modelo de dominio interno —en español, de TFU3— y esta
forma del frontend).

IMPORTANTE: el modelo de dominio interno NO se cambia. Solo se agrega traducción
en la capa de esquemas/DTOs.

## Tipos que usa el frontend

```ts
interface Product {
  id: string;
  name: string;        // <-> Producto.nombre
  price: number;       // <-> Producto.precio
  category: string;    // NUEVO: cosmético de UI (Combos, Hamburguesas, Papas, Bebidas)
  image: string;       // cosmético de UI (emoji/url)
  color: string;       // cosmético de UI
}

interface OrderItem {
  product: Product;
  quantity: number;    // <-> ItemPedido.cantidad
}

interface Order {
  id: number;
  number: number;          // <-> Pedido.numero
  customerName: string;    // <-> Pedido.nombre_cliente
  nickname: string;        // apodo (campo del front; puede mapearse a nombre_cliente o ignorarse)
  items: string[];         // el front lista items como texto; la API expone items estructurados
  status: 'casi-listo' | 'preparacion' | 'cola' | 'recibido';
  estimatedTime: number;   // cosmético de UI (minutos estimados)
  color: string;           // cosmético
  icon: string;            // cosmético
}
```

## Productos hardcodeados en el prototipo (menú real a sembrar)

| id  | name                  | price | category     |
|-----|-----------------------|-------|--------------|
| 1   | Combo 1               | 290   | Combos       |
| 2   | Hamburguesa con queso | 220   | Hamburguesas |
| 3   | Papas grandes         | 150   | Papas        |
| 4   | Coca Light            | 110   | Bebidas      |
| 5   | Wrap veggie           | 210   | Papas        |
| 6   | Limonada              | 120   | Bebidas      |

Pedidos frecuentes (atajos de UI, opcionales): Combo clásico (290),
Burger + papas (370), Papas + Coca (260), Wrap + limonada (330).

## Mapeo de estados (UI del front  <->  EstadoPedido del dominio TFU3)

| status (front) | label visible    | EstadoPedido (dominio) |
|----------------|------------------|------------------------|
| cola           | EN COLA          | REGISTRADO             |
| preparacion    | EN PREPARACIÓN   | EN_PREPARACION         |
| casi-listo     | CASI LISTO       | EN_PREPARACION (matiz visual; ver nota) |
| recibido       | LISTO/RECIBIDO   | LISTO                  |

Nota: 'casi-listo' es un matiz visual del front dentro de "en preparación".
En el dominio se mapea a EN_PREPARACION. El estado ENTREGADO del dominio no
tiene equivalente visual directo en la pantalla actual (el pedido desaparece
de la lista al entregarse); se puede exponer como 'recibido' o filtrarse.

## Regla de diseño para la API

- El dominio interno (Domain/) y la máquina de estados siguen usando los
  enums en español de TFU3: REGISTRADO, EN_PREPARACION, LISTO, ENTREGADO, CANCELADO.
- Los DTOs de salida (Dtos/) ofrecen DOS vistas:
  1. la "canónica" (español, ya existente), y
  2. una vista compatible con el front (campos en inglés + status kebab),
     vía un mapper, para que el prototipo pueda consumir la API sin reescribirse.
- La vista compatible se expone en `/frontend/menu`, `/frontend/pedidos` y
  `/frontend/pedidos/{numero}`.
- Los campos puramente cosméticos (image, color, icon, estimatedTime, category)
  son calculados por `FrontendMapper` con valores por defecto. No se modela
  lógica de negocio sobre ellos.
