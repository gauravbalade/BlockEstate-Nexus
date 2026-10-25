# BlockEstate Nexus
### Smart Property Registration · Ownership Verification · Fraud-Risk Review

BlockEstate Nexus is a final-year engineering project concept for clearer, traceable property-registration workflows. It brings property records, application review, document checklists, ownership history, risk indicators and audit events into one interface.

> **Honest status:** this repository is an engineering foundation and interactive prototype. The UI uses fictional demo data unless configured to call the API. The API now has SQL-backed property, registration-application and audit-event endpoints, but it is still a development scaffold without authentication. OCR, trained fraud models, blockchain anchoring, production QR verification, payment settlement and government integrations are not live until implemented and tested.

## Structure
- `frontend/` — React, TypeScript and Vite UI.
- `backend/BlockEstate.Api/` — ASP.NET Core Web API, EF Core and SQL Server.
- `database/schema.sql` — initial relational schema reference.
- `docs/` — architecture, safety boundaries and roadmap.

## Technology
React + TypeScript + Vite; ASP.NET Core Web API; Entity Framework Core; SQL Server.
The API targets **.NET 10 LTS**. The submitted proposal mentioned .NET 8; Microsoft lists .NET 8 end of support as **10 November 2026**, so confirm this framework change with the project guide before formal submission. See [Microsoft's support policy](https://dotnet.microsoft.com/en-us/platform/support/policy).

## Windows setup and smoke testing

For the full Windows/PowerShell setup, SQL Server schema preparation, API origin configuration, smoke-test endpoints and presentation safety checklist, see [`docs/WINDOWS_SETUP_AND_SMOKE_TEST.md`](docs/WINDOWS_SETUP_AND_SMOKE_TEST.md). Follow it before claiming that the UI is connected to the database.

## Run the frontend
Requires Node.js 20.19+ or 22.12+.
```bash
cd frontend
npm install
npm run dev
```

## Run the API
Requires .NET 10 SDK and SQL Server.
```bash
cd backend/BlockEstate.Api
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Registry" "Server=(localdb)\\MSSQLLocalDB;Database=BlockEstateNexus;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run
```
Before first API use, run `database/schema.sql` against your development SQL Server database. Optionally run `database/seed-demo.sql` to insert clearly fictional demonstration records. Then set the `Registry` connection string. The API does not auto-create tables.

Set `VITE_API_BASE_URL` in `frontend/.env` to the API base URL to enable API fetching. Without it, the UI uses clearly labelled local demo data.

## Trust and security boundaries
- Sample records are fictional.
- A QR code is a locator, not proof of title.
- Risk flags are review prompts, not fraud verdicts.
- Store sensitive legal documents in protected off-chain storage.
- If a permissioned ledger is added, anchor minimal integrity references only.
- Server-side authentication/authorisation and security testing are required before real-world use.

## Team
A 05 — Gaurav Bhimrao Balade · A 10 — Chaitanya Ramesh Bote · A 20 — Kunal Santa Das · A 49 — Yash Yogesh Kulkarni

Datta Meghe College of Engineering · Department of Computer Engineering · AY 2026–27
