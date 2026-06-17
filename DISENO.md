# Diseño y trazabilidad — UT5 · TFU

Este documento explica **qué se construyó**, **qué patrones se aplicaron y por
qué**, y **qué se modificó respecto de las unidades anteriores (UT1–UT4)**.
Es el hilo que conecta los eventos/casos de uso del negocio ya relevados con la
API DEMO entregada.

---

## 1. Alcance: dos casos de uso "caminando"

> Nota sobre numeración: a lo largo de UT2 y UT4 conviven dos numeraciones de
> PUC (la de la presentación de UT2 y la del prototipo de UT4). Para evitar
> ambigüedad, en esta entrega nos referimos a los casos de uso **por el nombre
> de la pantalla**, no por su número. Lo importante es que son los dos casos
> elegidos por su valor para la solución que pide el negocio.

Esta entrega implementa las **dos pantallas ya prototipadas en Figma (UT4)**,
por ser el núcleo mínimo demostrable y el de mayor valor para la defensa:

| Pantalla (UT4) | Qué hace | Requerimientos de UT2 que satisface |
|---|---|---|
| **"Crear Pedido"** (cajero) | El cajero ve el menú, selecciona productos y registra el pedido | RNF-01 (consultar menú independiente), RF-02 (seleccionar productos), RF-03 (registrar con estado), RF-04 (modificar en construcción) |
| **"Estado de Pedidos"** | Muestra pedidos por estado y permite avanzarlos | RF-07 (identificar pendientes), RNF-04 (actualizar estado de forma fluida) |

Esta tabla es la **trazabilidad hacia atrás**: cada pantalla/endpoint se ata a
requerimientos concretos que el equipo ya defendió y aprobó en la presentación
de UT2. Eso responde al pedido del enunciado de UT5 de *asegurar coherencia
entre el modelo de clases, el prototipo y la implementación*.

Los demás casos (cobro, retiro con verificación de código, reclamo) quedan
**documentados pero fuera del núcleo** de esta entrega, en línea con su
priorización original (p. ej. el reclamo fuera del punto de atención ya estaba
marcado como *Could Have* en UT2).

---

## 2. Trazabilidad: pantalla → endpoint → patrón

| Pantalla (UT4) | Endpoint | Patrón aplicado | Por qué el patrón sirve a este caso |
|---|---|---|---|
| "Crear Pedido" (soporte) | `GET /menu` | Repository | Desacopla el origen del catálogo; el menú se sirve sin que el servicio sepa de dónde sale. |
| "Crear Pedido" | `POST /pedidos` | **Builder** | El pedido se arma incrementalmente (agregar productos, sumar cantidades, nota opcional), igual que en la pantalla. Evita el constructor con muchos parámetros opcionales. |
| "Estado de Pedidos" | `GET /pedidos?estado=...` | Repository | Permite filtrar pedidos por estado para alimentar las columnas de la pantalla. |
| "Estado de Pedidos" | `PATCH /pedidos/{n}/estado` | **Máquina de estados (State)** | Centraliza qué transiciones del ciclo de vida son válidas; impide estados imposibles. |

---

## 3. Patrones aplicados (justificación)

### 3.1 Repository — *de TFU3, ahora implementado*
Ya figuraba como `PedidoRepository` en el diagrama de clases de UT3. Aquí se
materializa como **interfaz abstracta** (las interfaces en `Repositories/IRepositories.cs`) más una
**implementación in-memory** (`Repositories/InMemoryPedidoRepository.cs`). Los servicios
dependen de la interfaz, no de la implementación → **Dependency Inversion**
(la *D* de SOLID). Cambiar a SQLite sería agregar otra implementación sin tocar
los servicios.

### 3.2 Builder — *núcleo de PUC-01, conecta con el TA1 de UT5*
`PedidoBuilder` (`Patterns/PedidoBuilder.cs`) construye el pedido paso a
paso. Es el mismo problema y la misma solución que el **Trabajo de Aplicación 1
de UT5** (ejercicios del *Sandwich* y del *TravelPlan*, que sufren el
"telescoping constructor"). Lo aplicamos donde el negocio realmente arma algo
de forma incremental: la creación del pedido por el cajero.

### 3.3 Máquina de estados (State) — *núcleo de PUC-02*
`MaquinaEstadosPedido` (`Patterns/MaquinaEstadosPedido.cs`) define las transiciones
válidas de `EstadoPedido` en un único lugar. Garantiza el ciclo de vida del
pedido que la pantalla "Estado de Pedidos" hace visible y evita condicionales
repartidos por el código (favorece **Open/Closed**).

### 3.4 Capas de servicio + DTOs — *SOLID / MVC*
- El **dominio** (`Domain/`, clases C#) no conoce HTTP ni validación.
- Los **DTOs** (`Dtos/`, records C#) manejan validación y serialización.
- Los **servicios** (`Services/`) tienen la lógica de aplicación.
- Los **controllers** (`Controllers/`) solo traducen HTTP ↔ servicio.
- `FrontendController` y `FrontendMapper` agregan una fachada compatible con
  Figma Make sin contaminar el modelo de dominio con datos cosméticos.

Esta separación es la aplicación concreta de **SRP** y del espíritu de **MVC**
trabajado en el Trabajo de Aplicación 2 de UT5.

---

## 4. Qué se modificó respecto de UT1–UT4

> Esta sección es la respuesta directa al pedido de la consigna: dejar claro
> **qué se logró y qué se modificó** de lo conseguido en unidades anteriores.

1. **De casos de uso del negocio (BUC, UT2) a casos de uso ejecutables.** En
   UT1/UT2 los casos eran descripciones en lenguaje natural. En esta entrega,
   las pantallas "Crear Pedido" y "Estado de Pedidos" pasaron a ser **endpoints
   reales** que se pueden invocar y probar.

2. **El identificador de pedido se concretó como número incremental.** UT2
   asumía "un identificador simple, como número o nombre". La API lo fija como
   un `numero` autoincremental que funciona como identificador de retiro y como
   clave de los endpoints `/pedidos/{numero}`.

3. **El estado del pedido pasó de concepto a máquina de estados validada.** El
   enum `EstadoPedido` venía de UT3, pero las transiciones estaban implícitas.
   Ahora son **explícitas y validadas**, y se exponen los "estados siguientes"
   en cada respuesta para que el frontend sepa qué acciones ofrecer.

4. **Repository dejó de ser una caja en el diagrama y pasó a ser una abstracción
   real** con su implementación, habilitando el cambio de persistencia futuro.

5. **Ajuste de actor en la pantalla "Crear Pedido".** En la documentación de
   UT2, el caso de registrar/consultar el pedido aparecía con el cliente como
   actor en algún punto; el prototipo de UT4 y esta API lo modelan con el
   **cajero** operando la creación del pedido (el cliente consulta el estado),
   que refleja mejor la operatoria real relevada.

6. **Se agregó una fachada para el frontend externalizado.** Los endpoints
   `/frontend/menu` y `/frontend/pedidos` traducen entre el contrato canónico
   de la API y los campos que usa el prototipo Figma Make (`name`, `status`,
   `estimatedTime`, etc.). Esta capa concreta el rol de la API como herramienta
   de comunicación con un equipo de frontend externo.

7. **El diagrama de clases de UT3 queda sujeto a revisión** a partir de lo que
   la API introdujo (Builder, máquina de estados explícita, capa de mapeo a la
   forma del frontend). Esa revisión se documenta por separado, una vez
   estabilizada la API, según pide el punto 3 del enunciado de UT5.

8. **Se mantuvo deliberadamente fuera del núcleo** el cobro, el retiro con
   verificación y el reclamo, para entregar algo mínimo y sólido. Quedan como
   evolución natural (el modelo de TFU3 ya tiene `Pago`, `PagoService`,
   `TicketRetiro`, etc. listos para incorporarse).

---

## 5. Posible evolución (no incluida en esta entrega)

- **Cobro:** incorporar `Pago` + un **patrón Strategy** para los medios de pago
  (electrónico vía `SistemaPagoExterno` / efectivo), que es exactamente la rama
  `alt` del diagrama de secuencia de TFU3.
- **Retiro:** verificación del código de retiro antes de marcar `ENTREGADO`.
- **Notificación "pedido listo":** `NotificacionService` como **Observer** del
  cambio de estado a `LISTO`.
- **Persistencia real:** implementación SQLite del Repository.
