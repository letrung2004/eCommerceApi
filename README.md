# E-Commerce — API Sàn thương mại điện tử Microservices

[![Frontend](https://img.shields.io/badge/Frontend-React_TypeScript-blue)]()
[![Backend](https://img.shields.io/badge/Backend-.NET_8_&_C%23-brightgreen)]()
[![gRPC](https://img.shields.io/badge/gRPC-HTTP2-purple)]()
[![Message Bus](https://img.shields.io/badge/Message_Bus-MassTransit%20%26%20RabbitMQ-orange)]()
[![Database](https://img.shields.io/badge/Database-MongoDB%2C_PostgreSQL%2C_SQLServer-green)]()
[![Cache](https://img.shields.io/badge/Cache-Redis-yellow)]()
[![Identity](https://img.shields.io/badge/Identity-Duende_JWT_RS256-red)]()
[![Architecture](https://img.shields.io/badge/Architecture-Clean_DDD%2C_CQRS%2C_Saga-red)]()
[![Deployment](https://img.shields.io/badge/Deployment-Docker_&_Nginx-lightgrey)]()

## 🔹 Project Highlights

### Gateway
- YARP (.NET 8) + Rate Limiting + Output Caching
- OpenAPI aggregation, request/response logging (Serilog + OpenTelemetry)

### Identity
- Duende IdentityServer, JWT (RS256), refresh token rotation
- Policy + Scope/role-based authorization
- Password hashing: ASP.NET Core Identity + Argon2/PBKDF2
- BFF support for web if cookie-based auth needed

### ProductService
- MongoDB (collection-per-aggregate), text/compound indexes, soft-delete
- Minimal API + Vertical Slice
- Filtering/paging/sorting, publish `product.price-updated` event

### Basket
- Redis Hash/JSON (StackExchange.Redis), TTL per user

### Inventory
- PostgreSQL / SQL Server + EF Core
- gRPC (sync) for fast stock check; RabbitMQ for `inventory.reserved` / `inventory.released`
- Reserve/Release logic with Quartz/BackgroundService for timeout handling

### Ordering
- CQRS: commands (SQL + EF Core) / queries (read model)
- Outbox + Inbox pattern for exactly-once message delivery
- MassTransit + RabbitMQ (retry, DLQ, saga, scheduling)
- OrderingSaga: MassTransit Saga State Machine for orchestration
- Idempotency-Key header + optimistic concurrency

### Payment
- Sync calls (gRPC/HTTP) for pre-authorization
- Final capture via events (`payment.captured` / `payment.failed`)

### Orchestrator
- Saga orchestration with MassTransit
- Flow: `order.created` → Reserve Inventory → Pre-authorize Payment → Confirm → `order.completed`
- Error branch: Release Inventory + `order.canceled`


## 🏗️ Full System Architecture

```mermaid
flowchart TD
    %% Users
    User[User / Client]

    %% Gateway
    Gateway[YARP Gateway\nRate Limiting + Caching\nOpenAPI aggregation]

    %% Services
    Identity[IdentityService\nDuende IdentityServer\nJWT RS256 + Refresh Token]
    Product[ProductService\nMongoDB + Vertical Slice API\nproduct.price-updated event]
    Basket[BasketService\nRedis Hash/JSON + TTL per user]
    Inventory[InventoryService\nPostgreSQL/SQL Server + EF Core\ngRPC + RabbitMQ\nReserve/Release logic]
    Ordering[OrderService\nCQRS + Outbox/Inbox + MassTransit + SagaStateMachine]
    Payment[PaymentService\ngRPC/HTTP PreAuthorize + Event-based Capture]

    %% Saga Worker
    SagaWorker[OrderSaga.Worker\nMassTransit Saga Orchestrator]

    %% Databases / Cache
    MongoDB[(MongoDB)]
    SQLDB[(PostgreSQL / SQL Server)]
    Redis[(Redis)]
    
    %% Events
    OrderCreated["OrderCreatedEvent\n(RabbitMQ)"]
    InventoryReserved["InventoryReservedEvent\n(RabbitMQ)"]
    InventoryReleased["InventoryReleasedEvent\n(RabbitMQ)"]
    PaymentAuthorized["PaymentAuthorizedEvent\n(RabbitMQ)"]
    PaymentCaptured["PaymentCapturedEvent\n(RabbitMQ)"]
    PaymentFailed["PaymentFailedEvent\n(RabbitMQ)"]
    OrderConfirmed["OrderConfirmedEvent\n(RabbitMQ)"]
    OrderCancelled["OrderCancelledEvent\n(RabbitMQ)"]

    %% User → Gateway
    User -->|HTTP Requests| Gateway

    %% Gateway → Services
    Gateway -->|Auth Requests| Identity
    Gateway -->|API Requests| Product
    Gateway -->|Basket Requests| Basket
    Gateway -->|Order Requests| Ordering

    %% Service DB connections
    Product --> MongoDB
    Ordering --> SQLDB
    Inventory --> SQLDB
    Basket --> Redis
    Identity --> SQLDB

    %% Ordering flow
    Ordering -->|Publish| OrderCreated
    OrderCreated --> SagaWorker

    %% Saga orchestrator calls
    SagaWorker -->|gRPC| Inventory
    Inventory -->|InventoryReservedEvent| SagaWorker
    Inventory -.->|InventoryReleasedEvent| SagaWorker

    SagaWorker -->|gRPC| Payment
    Payment -->|PaymentAuthorizedEvent| SagaWorker
    Payment -->|PaymentCapturedEvent| SagaWorker
    Payment -.->|PaymentFailedEvent| SagaWorker

    %% Saga final events
    SagaWorker --> OrderConfirmed
    SagaWorker -.-> OrderCancelled

