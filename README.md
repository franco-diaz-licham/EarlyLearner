# 🌱 EarlyLearner

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=111111)
![TypeScript](https://img.shields.io/badge/TypeScript-6-3178C6?logo=typescript&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?logo=tailwindcss&logoColor=white)

EarlyLearner is a full-stack early childhood learning record and school-readiness application. It helps carers manage households and children, capture daily learning moments, attach evidence, and track school-readiness outcomes.

The project is built as a cloud-native, event-driven .NET system using a modular API, background worker, Azure-managed services, and a React frontend.

---

## ✨ Features

- Household and child profiles
- Readiness outcome catalogue
- Child readiness profiles and progress tracking
- Daily logs with learning moments and file evidence
- Readiness evidence modelling
- Household invitation email workflow
- Realtime invitation delivery status notifications
- Dashboard query slice for household summary data

---

## 🧰 Technology Stack

### ☁️ Azure-Native Services

- **Azure Container Apps** for the API and worker services
- **Azure Static Web Apps** for the React frontend
- **Azure Container Registry** for application images
- **Azure Service Bus Namespace** for topic-based integration events
- **Azure Cosmos DB** for short-lived document storage
- **Azure Database for PostgreSQL** for relational application data
- **Azure Communication Services** for email delivery
- **Azure Storage Account** for file/blob storage
- **Azure Key Vault** for secrets and protected configuration
- **Microsoft Entra External ID** tenant for customer identity
- **Application Insights** for application telemetry
- **Log Analytics workspace** for centralized logs

### ⚙️ Backend

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL with Npgsql
- Azure Service Bus with MassTransit
- Azure Cosmos DB document store
- Clean Architecture project split
- Domain-driven design style entities and value objects
- CQRS-style application commands and queries
- Event-driven integration between API and worker processes
- Serilog
- Swagger / OpenAPI

### 🖥️ Frontend

- React 19
- Vite
- TypeScript
- Tailwind CSS 4
- PrimeReact
- Unicons
- TanStack Query
- React Router
- Zustand
- React Hook Form
- Zod
- Vitest and React Testing Library

### 🐳 Local Infrastructure

- Docker Compose
- PostgreSQL 17
- Azure Cosmos DB emulator
- Azurite for Azure Blob Storage emulation
- Azure Service Bus emulator
- OpenTelemetry collector
- PowerShell helper scripts

---

## 🏗️ Project Architecture

### Backend

```text
backend/
├── EarlyLearner.Api/             # Minimal API endpoints, app startup, Swagger, HTTP concerns
├── EarlyLearner.Application/     # Use-case contracts, query/command results, application ports
├── EarlyLearner.Domain/          # Entities, value objects, enums, domain events, business rules
├── EarlyLearner.Infrastructure/  # EF Core DbContext, configurations, migrations, external adapters
├── EarlyLearner.Worker/          # Message consumers, email delivery, worker-owned audit persistence
└── EarlyLearner.Shared/          # Shared helpers, document-store abstractions, cross-process options
```

### Frontend

```text
frontend/
├── src/
│   ├── app/       # App shell, routing, providers
│   ├── features/  # Feature modules and user-facing workflows
│   ├── shared/    # Shared UI, config, services, utilities
│   ├── test/      # Test setup
│   └── types/     # Shared TypeScript declarations
└── package.json
```

### Docker

```text
docker/
├── docker-compose.yml
├── api.dockerfile
├── worker.dockerfile
└── db-seeder.dockerfile
```

Azure Service Bus cloud setup is documented in `docs/azure-service-bus.md`.

---

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- Docker Desktop
- PowerShell 7+
- PostgreSQL client tools if running `scripts/init-db.ps1` directly from the host

---

## 🐳 Run With Docker

From the repository root:

```powershell
./scripts/compose.ps1
```

This stops the current compose environment, rebuilds images, and starts the local stack using:

```text
docker/docker-compose.yml
```

Main local services:

```text
API:         http://localhost:5136
Worker:      early-learner-worker container
PostgreSQL:  localhost:55432
Audit DB:    localhost:55433
Cosmos DB:   https://localhost:8081
Azurite:     localhost:10000
Service Bus: localhost:5672
OTLP:        localhost:4317
```

Docker database credentials:

```text
Database: early_learner_main
Username: local-dev
Password: local-dev
```

---

## ⚙️ Backend Setup

For running the API locally against the Docker PostgreSQL container, use this connection string:

```json
{
    "ConnectionStrings": {
        "Db": "Host=127.0.0.1;Port=55432;Database=early_learner_main;Username=local-dev;Password=local-dev"
    }
}
```

Apply migrations:

```powershell
dotnet ef database update --project backend/EarlyLearner.Infrastructure --startup-project backend/EarlyLearner.Api
```

Run the API locally:

```powershell
dotnet run --project backend/EarlyLearner.Api
```

Build the API:

```powershell
dotnet build backend/EarlyLearner.Api/EarlyLearner.Api.csproj
```

Run the worker locally:

```powershell
dotnet run --project backend/EarlyLearner.Worker
```

Build the worker:

```powershell
dotnet build backend/EarlyLearner.Worker/EarlyLearner.Worker.csproj
```

When running API or worker outside Docker, keep the emulator-backed settings aligned with `appsettings.json`:

```json
{
    "CosmosDb": {
        "ConnectionString": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;",
        "DatabaseName": "earlylearner",
        "DefaultTimeToLiveSeconds": 86400,
        "AllowInsecureEmulatorCertificate": true
    },
    "AzureServiceBus": {
        "ConnectionString": "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;",
        "AdministrationConnectionString": "Endpoint=sb://localhost:5300;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"
    }
}
```

---

## 🌱 Database Seeding

The Docker Compose stack includes a `db-seeder` service that runs:

```text
scripts/init-db.ps1
scripts/seed.sql
```

The seed script inserts a small predictable demo dataset with fixed UUIDs for:

- household
- carers
- children
- readiness outcomes
- readiness profiles
- daily logs
- learning moments
- readiness evidence

---

## 🖥️ Frontend Setup

From the frontend directory:

```powershell
cd frontend
npm install
npm run dev
```

Useful commands:

```powershell
npm run build
npm run lint
npm run test
npm run typecheck
```
