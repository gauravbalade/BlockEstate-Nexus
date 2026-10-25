# BlockEstate Nexus — Windows Setup and Demo Readiness

## Important boundary
This repository is an interactive academic prototype. The API currently has **no authentication or authorisation**. The current branch disables POST/PUT/PATCH/DELETE requests outside the Development environment as a fail-closed safeguard; this is not a substitute for proper identity policies, and read endpoints still must not expose real/sensitive data. Do not expose it publicly with real or sensitive data. Status transitions and risk indicators are workflow aids, not legal decisions or proof of title. Use fictional records only.

## Prerequisites
- Node.js 22 LTS (or a supported version listed in the README).
- .NET 10 SDK.
- SQL Server or SQL Server LocalDB (Visual Studio Installer can add LocalDB on Windows).
- PowerShell and a browser.

## 1. Get the code
Download this branch ZIP from GitHub and extract it, or clone the repository and check out `fixes/demo-readiness`.

## 2. Prepare SQL Server
Open SQL Server Management Studio (SSMS) or another SQL query tool and run `database/schema.sql` against the development SQL Server instance. Optionally run `database/seed-demo.sql` to load fictional sample rows. The API does not create its schema automatically.

If using LocalDB, confirm it exists with:
```powershell
sqllocaldb info
```

## 3. Configure and run the API
From the repository root in PowerShell:
```powershell
cd backend/BlockEstate.Api
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Registry" "Server=(localdb)\MSSQLLocalDB;Database=BlockEstateNexus;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run
```
Keep the terminal open. Use the HTTP/HTTPS URL printed by ASP.NET Core. The API's `/api/health` is liveness only; `/api/health/ready` checks database connectivity. Do not interpret a healthy liveness response as proof that SQL Server is reachable.

If LocalDB is not installed, replace the connection string with the SQL Server instance available on your computer. Do not commit credentials to Git.

## 4. Configure and run the frontend
Open a second PowerShell terminal from the repository root:
```powershell
cd frontend
npm ci
$env:VITE_API_BASE_URL="http://localhost:5000"
npm run dev
```
**Important:** replace `http://localhost:5000` with the actual HTTP URL printed by `dotnet run`. If the API prints only an HTTPS URL, use that URL and confirm the browser trusts its local development certificate. Do not guess the API port. The Vite URL is usually `http://localhost:5173`.

If `VITE_API_BASE_URL` is absent, the UI intentionally uses local fictional demo data. That mode does not demonstrate backend persistence. Confirm the browser network tab shows successful requests before claiming the API is connected.

## 5. Smoke-check endpoints
With the API running, open these routes using the actual API origin:
- `/api/health`
- `/api/health/ready`
- `/api/properties`
- `/api/applications`
- `/api/audit-events`

The data endpoints require the SQL schema to have been applied. Try creating one fictional application through the UI and confirm it appears after refreshing the page. Then change status only with fictional records and confirm an audit event is recorded.

## 6. Presentation checklist
- Use only the fictional seed data or newly entered fictional data.
- State clearly whether the UI is using local demo data or the SQL-backed API.
- Demonstrate a workflow only after it has been smoke-tested on the machine used for presentation.
- Describe OCR, trained fraud detection, blockchain anchoring, production QR verification, payment settlement, document storage and government integrations as future work unless separately implemented and verified.
- Do not use real IDs, deeds, addresses, payment credentials or OTPs.

## Current release blockers
- No API authentication/authorisation; write endpoints are not safe for public exposure.
- Database schema must be applied manually.
- No formal migration workflow or integration tests against a live SQL Server instance.
- No protected document upload/storage or malware scanning.
- API routes and UI-to-API integration need a local end-to-end smoke test.
- No legal authority or government integration is represented.

A successful CI build proves compilation/build only, not that the app is secure, connected to a database, or end-to-end ready.
