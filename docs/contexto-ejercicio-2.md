# Contexto: Ejercicio 2 — RabbitMQ (temporal, eliminar despues)

## Que se hizo hasta ahora

### Paso 1: Infraestructura
- Agregado servicio `rabbitmq` (imagen `rabbitmq:3-management`) a `docker-compose.yml`
- Puertos 5672 (AMQP) y 15672 (panel de admin) expuestos
- Variable `RabbitMQ__HostName=rabbitmq` en ordersservice (`docker-compose.override.yml`)

### Paso 2: NuGet
- `RabbitMQ.Client` v7.2.2 instalado en OrdersService

### Paso 3: Contrato compartido
- `src/Shared/Contracts/OrderCreatedEvent.cs` — record con OrderId, ProductId, Quantity, CreatedAt

### Paso 4: Publicador en OrdersService
- `IOrdersPublisher.cs` — interfaz
- `OrdersPublisher.cs` — implementacion con `QueueDeclareAsync` (durable=true) y `BasicPublishAsync`
- `Program.cs` — `IConnection` singleton + `IOrdersPublisher` singleton
- `OrdersService.cs` — inyecta `IOrdersPublisher`, publica despues de crear orden y descontar stock

### Paso 5: NotificationService (parcial)
- Proyecto Worker Service creado en Visual Studio
- `NotificationService.csproj` — SDK Worker, NuGet `RabbitMQ.Client` + `Microsoft.Extensions.Hosting`, ref Contracts
- `Program.cs` — `Host.CreateApplicationBuilder`, registra `IConnection` y `OrderCreatedWorker`
- `OrderCreatedWorker.cs` — BackgroundService, consumer con `autoAck: false`, `BasicAckAsync`

## Que falta hacer

### Paso 5 (completar):
1. Crear `Dockerfile` en `src/Services/NotificationService/` (multi-stage, base `dotnet/runtime:10.0`)
2. Verificar que compila

### Paso 6: Integrar en Docker Compose
1. Agregar `notificationservice` en `docker-compose.yml` con `depends_on: rabbitmq`
2. Agregar variables de entorno en `docker-compose.override.yml`

### Paso 7: Verificacion
1. `docker compose up -d --build`
2. Verificar 4 servicios corriendo: inventoryservice, ordersservice, rabbitmq, notificationservice
3. Abrir panel RabbitMQ (`localhost:15672`) — verificar que existe `orders_created_queue`
4. Crear una orden via POST /api/orders
5. Ver logs de notificationservice: `docker compose logs notificationservice`
6. Deberia mostrar: "Pedido recibido", "Enviando email", "Email enviado"

## Decisiones clave
- **Worker Service** en vez de ASP.NET Core: no necesita HTTP, solo corre en background
- **`dotnet/runtime`** en vez de `dotnet/aspnet`: imagen mas liviana, no necesita Kestrel
- **`autoAck: false`**: el mensaje se confirma manualmente con `BasicAckAsync` despues de procesarlo
- **`durable: true`** en cola + **`DeliveryMode.Persistent`** en mensajes: sobreviven reinicios de RabbitMQ
- **Declaracion de cola en ambos lados** (publisher y consumer): idempotente, funciona sin importar quien arranca primero
- **Singleton para `IConnection`**: la conexion TCP es costosa, se reutiliza; los canales se crean por operacion

## Arquitectura del flujo
```
Cliente -> POST /api/orders -> OrdersService
  -> HTTP -> InventoryService (verificar stock + descontar)
  -> AMQP -> RabbitMQ (orders_created_queue) -> NotificationService (consumer)
```
