# TODO

## Exercise 1 - Synchronous Communication with REST

- [x] Create solution and projects
- [x] Shared Contracts (DTOs, enums)
- [x] InventoryService - GET /api/inventory/{id}
- [x] OrdersService - POST /api/orders
- [x] OrdersService calls InventoryService via HttpClientFactory
- [x] SQLite persistence with seed data
- [x] Deduct stock on order creation
- [x] Docker Compose setup

**Optional (from guide):**
- [ ] Endpoints PUT/DELETE for inventory management
- [ ] Proper HTTP error codes and error handling

## Exercise 2 - Asynchronous Communication with RabbitMQ

- [x] Set up RabbitMQ with Docker Compose
- [x] OrdersService publishes "Order Created" message
- [x] NotificationService consumes messages
- [x] NotificationService sends email (simulated)

**Optional (from guide):**
- [x] Durable queues and persistent messages
- [x] Manual acknowledgments
- [ ] Multiple consumers (load balancing)
- [x] RabbitMQ monitoring panel

## Exercise 3 - Event Processing with Kafka

- [ ] Set up Kafka + Zookeeper with Docker Compose
- [ ] Publish order state changes (created, confirmed, shipped) to Kafka topic
- [ ] NotificationService consumes from Kafka
- [ ] New AnalyticsService consumes from Kafka

**Optional (from guide):**
- [ ] Partitions and replication
- [ ] Manual offsets and confirmations
- [ ] Multiple consumers for parallelism
