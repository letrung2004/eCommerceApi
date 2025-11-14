# E-Commerce — API Sàn Thương Mại Điện Tử Microservices

[![Frontend](https://img.shields.io/badge/Frontend-React_TypeScript-blue)]()
[![Backend](https://img.shields.io/badge/Backend-.NET_8_&_C%23-brightgreen)]()
[![gRPC](https://img.shields.io/badge/gRPC-HTTP2-purple)]()
[![Message Bus](https://img.shields.io/badge/Message_Bus-MassTransit%20%26%20RabbitMQ-orange)]()
[![Database](https://img.shields.io/badge/Database-MongoDB%2C_PostgreSQL%2C_SQLServer-green)]()
[![Cache](https://img.shields.io/badge/Cache-Redis-yellow)]()
[![Identity](https://img.shields.io/badge/Identity-Duende_JWT_RS256-red)]()
[![Architecture](https://img.shields.io/badge/Architecture-Clean_DDD%2C_CQRS%2C_Saga-red)]()
[![Deployment](https://img.shields.io/badge/Deployment-Docker_&_Nginx-lightgrey)]()

---

## Project Highlights

### Gateway
- YARP (.NET 8) với Rate Limiting + Output Caching
- OpenAPI aggregation, request/response logging (Serilog + OpenTelemetry)

### Identity
- Duende IdentityServer, JWT (RS256), refresh token rotation
- Policy + Scope/Role-based authorization
- Password hashing: ASP.NET Core Identity + Argon2/PBKDF2
- BFF support cho web nếu cần cookie-based auth

### ProductService
- MongoDB (collection-per-aggregate), text/compound indexes, soft-delete
- Minimal API + Vertical Slice
- Filtering/paging/sorting, publish event `product.price-updated`

### Basket
- Redis Hash/JSON (StackExchange.Redis), TTL per user

### Inventory
- PostgreSQL / SQL Server + EF Core
- gRPC (sync) để kiểm tra stock nhanh; RabbitMQ cho `inventory.reserved` / `inventory.released`
- Reserve/Release logic với Quartz/BackgroundService để xử lý timeout

### Ordering
- CQRS: commands (SQL + EF Core) / queries (read model)
- Outbox + Inbox pattern đảm bảo exactly-once message delivery
- MassTransit + RabbitMQ (retry, DLQ, saga, scheduling)
- OrderingSaga: MassTransit Saga State Machine cho orchestration
- Idempotency-Key header + optimistic concurrency

### Payment
- Sync calls (gRPC/HTTP) cho pre-authorization
- Final capture thông qua events (`payment.captured` / `payment.failed`)

### Orchestrator
- Saga orchestration với MassTransit
- Flow: `order.created` → Reserve Inventory → Pre-authorize Payment → Confirm → `order.completed`
- Error branch: Release Inventory + `order.canceled`

---

## Architecture Overview


- **Communication**: gRPC (sync), RabbitMQ (async)
- **Patterns**: Clean Architecture, DDD, CQRS, Saga, Outbox/Inbox
- **Deployment**: Docker + Nginx + optional Kubernetes

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- RabbitMQ, Redis, PostgreSQL/MongoDB running (can use Docker Compose)

### Run Services
```bash
# Run all services via Docker Compose
docker-compose up --build

# Run individual service
cd src/Services/ProductService
dotnet run
