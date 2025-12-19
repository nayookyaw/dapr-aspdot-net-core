# Dapr and ASP.Net Core API (Catalog & Checkout)
This project demonstrates how to run a .NET Catalog & Checkout API alongside the [Dapr](https://dapr.io) sidecar runtime.  
Dapr provides building blocks for service invocation, state management, pub/sub, and more.

---

## Catalog API with Dapr

## 📦 Prerequisites

- [.NET 9+ SDK](https://dotnet.microsoft.com/download)
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
- Docker (optional, if your Dapr components depend on containers like Redis)

---

## 🚀 Running the Catalog API with Dapr

1. **Initialize Dapr (only once per machine):**
   ```bash
   dapr init

2. Terminal 1 (Powershell): CatalogApi + Dapr sidecar
    <i>
    dapr run `
  --app-id catalog-api `
  --app-port 5001 `
  --dapr-http-port 3501 `
  --resources-path .\components `
  -- dotnet run --project .\CatalogApi\CatalogApi.csproj --urls http://localhost:5001
    <i>

3. Terminal 2 (Powershell): CheckoutApi + Dapr sidecar
    <i>
    dapr run `
  --app-id checkout-api `
  --app-port 5002 `
  --dapr-http-port 3502 `
  --resources-path .\components `
  -- dotnet run --project .\CheckoutApi\CheckoutApi.csproj --urls http://localhost:5002
    </i>