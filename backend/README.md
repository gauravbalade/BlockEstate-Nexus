# Backend API

Initial ASP.NET Core Web API using EF Core and SQL Server.

- `GET /api/health` — service health (not a database readiness check).
- `GET /api/properties?search=&status=` — search/filter persisted records.
- `GET /api/properties/{propertyCode}` — property lookup.
- `POST /api/properties` — create a property with basic validation.
- `PUT /api/properties/{propertyCode}/status` — update to an allowed status.

Configure `ConnectionStrings:Registry` through user secrets or `ConnectionStrings__Registry`. Configure `Cors:AllowedOrigins` for the frontend origin.

Not production-ready: no authentication/authorisation, formal application workflow, document upload/storage, persistent audit log, rate limiting, OCR, ML or blockchain integration. Do not expose publicly with real data until security controls and tests are added.
