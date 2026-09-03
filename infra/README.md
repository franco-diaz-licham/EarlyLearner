# Azure Infrastructure

This folder provisions the Azure resources used by the production-shaped EarlyLearner deployment with the cheapest viable Azure plans:

- Azure Static Web Apps Free for the React/Vite frontend.
- Azure Container Apps consumption for the API and worker, with zero minimum replicas and one maximum replica by default.
- Azure Container Registry Basic for application images.
- Azure Database for PostgreSQL Flexible Server on Burstable `Standard_B1ms`, with one server hosting both the main and audit databases.
- Azure Service Bus Standard for topic-based integration events. Basic is cheaper, but it does not support topics and subscriptions.
- Azure Cosmos DB SQL API with database-level 400 RU/s provisioned throughput and free tier enabled by default.
- Azure SignalR Service Free_F1 in Default mode for API-hosted realtime hubs.
- Azure Storage Standard_LRS for uploaded file evidence.
- Azure Communication Services, Email Communication Service, and an Azure managed email domain.
- Application Insights connected to Log Analytics with 30-day retention.
- Key Vault containing generated runtime connection strings and secrets.

Regional resources default to `australiaeast`. Static Web Apps defaults to `eastasia`, matching the nearby low-cost region used by the other projects.

`main.bicep` is intentionally thin. It owns parameters, naming, and wiring between modules. Product-specific resources live under `modules/`.

## Deploy

Create `infra/.env` from `infra/.env.example` and set the environment values:

```dotenv
AZURE_SUBSCRIPTION=<subscription-id-or-name>
AZURE_RESOURCE_GROUP_NAME=rg-earlylearner-prod
AZURE_RESOURCE_GROUP_LOCATION=australiaeast
AZURE_DEPLOYMENT_NAME=earlylearner-infra
AZURE_TEMPLATE_FILE=infra/main.bicep
AZURE_PARAMETERS_FILE=infra/main.parameters.prod.json

POSTGRES_ADMIN_PASSWORD=<strong-postgresql-admin-password>
ENCRYPTION_KEY=<base64-encoded-32-byte-key>

AZURE_AD_INSTANCE=<identity-instance-url>
AZURE_AD_TENANT_ID=<tenant-id>
AZURE_AD_CLIENT_ID=<api-app-client-id>
FRONTEND_ENTRA_CLIENT_ID=<frontend-app-client-id>
FRONTEND_ENTRA_AUTHORITY=<frontend-authority-url>
FRONTEND_ENTRA_API_SCOPE=<api-scope>

COMMUNICATION_SENDER_ADDRESS=
AZURE_SKIP_WHAT_IF=false
AZURE_WHAT_IF_ONLY=false
```

Run the provisioning script from the repository root:

```powershell
./scripts/provision-infra.ps1
```

Useful variants:

```powershell
./scripts/provision-infra.ps1 -Subscription "<subscription-id-or-name>"
./scripts/provision-infra.ps1 -WhatIfOnly
./scripts/provision-infra.ps1 -SkipWhatIf
```

The script creates the resource group, merges committed Bicep parameters with local `.env` values, deploys `main.bicep`, and prints the GitHub secrets needed by the existing workflows.

## Parameters And Secrets

Normal infrastructure values live in `main.parameters.prod.json`. Runtime identity values and secrets live in ignored `infra/.env` so they can differ between environments without editing the committed parameters.

The Azure managed email domain generates a sender domain. Leave `COMMUNICATION_SENDER_ADDRESS` blank to configure the worker with `DoNotReply@<generated-domain>.azurecomm.net`. Set it only after a custom sender address is verified.

Cosmos DB free tier can only be enabled on one account per subscription. If the target subscription already has a free-tier Cosmos account, set `cosmosEnableFreeTier` to `false` before deploying.

## GitHub Secrets

After deployment, map the `githubSecrets` output to repository secrets:

- `ACR_NAME`
- `API_BASE_URL`
- `API_CONTAINER_APP_NAME`
- `AZURE_RESOURCE_GROUP`
- `ENTRA_API_SCOPE`
- `ENTRA_AUTHORITY`
- `ENTRA_CLIENT_ID`
- `RESOURCE_GROUP`
- `SWA_NAME`
- `SWA_RESOURCE_GROUP`
- `WORKER_CONTAINER_APP_NAME`

`AZURE_CREDENTIALS` still comes from your existing service-principal setup.

## Deprovision

To delete the Azure environment, run:

```powershell
./scripts/deprovision-infra.ps1
```

The script reads the subscription and resource group from `infra/.env`, lists the resources that will be deleted, and asks you to type the resource group name before deletion.

To skip the confirmation prompt or run asynchronously:

```powershell
./scripts/deprovision-infra.ps1 -Force
./scripts/deprovision-infra.ps1 -Force -NoWait
```

## Notes

The deployment creates placeholder Container Apps images. The existing GitHub Actions workflows push the real API and worker images to ACR and update the Container Apps after provisioning.

The cheapest defaults trade cost for cold starts and limited scale. API and worker both start at zero replicas, run on small CPU/memory allocations, and cap at one replica unless the parameters are changed.
