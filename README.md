# 🌱 EarlyLearner

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=111111)
![TypeScript](https://img.shields.io/badge/TypeScript-6-3178C6?logo=typescript&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?logo=tailwindcss&logoColor=white)

EarlyLearner is a full-stack early childhood learning record and school-readiness application. It helps carers manage households and children, capture daily learning moments, attach evidence, and track school-readiness outcomes.

The project is built as a technical showcase for a production-style modular monolith: a .NET API with Clean Architecture boundaries, EF Core persistence, Azure-native messaging, Docker-based local infrastructure, and a modern React frontend.

---

## ✨ Features

- Household and child profiles
- Readiness outcome catalogue
- Child readiness profiles and progress tracking
- Daily logs with learning moments and file evidence
- Readiness evidence modelling
- Azure Service Bus integration messaging with EF outbox
- Dashboard query slice for household summary data
- Dockerized local infrastructure for PostgreSQL, Azurite, Azure Service Bus emulator, API, worker, and database seeding

---

## 🧰 Technology Stack

### ⚙️ Backend

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL with Npgsql
- Clean Architecture project split
- Domain-driven design style entities and value objects
- CQRS-style application commands and queries
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
- Azurite for Azure Blob Storage emulation
- Azure Service Bus emulator
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
└── EarlyLearner.Shared/          # Shared helpers and cross-cutting utility code
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
├── early-learner-api.dockerfile
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
Azurite:     localhost:10000
Service Bus: localhost:5672
```

Docker database credentials:

```text
Database: early_learner
Username: local-dev
Password: local-dev
```

---

## ⚙️ Backend Setup

For running the API locally against the Docker PostgreSQL container, use this connection string:

```json
{
    "ConnectionStrings": {
        "Db": "Host=127.0.0.1;Port=55432;Database=early_learner;Username=local-dev;Password=local-dev"
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
