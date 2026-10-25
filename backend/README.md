# Backend API

ASP.NET Core Web API using EF Core and SQL Server. Apply `database/schema.sql` to a development database before using data endpoints; the app does not auto-create tables.

- `GET /api/health` — liveness response; does not check the database.
- `GET /api/health/ready` — checks database connectivity and returns 503 when SQL Server is unavailable.
- `GET /api/properties?search=&status=` and `GET /api/properties/{code}` — property lookup/search.
- `POST /api/properties` — create a property record.
- `GET /api/applications` — list registration applications with property names.
- `GET /api/applications/{code}` — fetch one application by its reference (the resource returned by the create endpoint).
- `POST /api/applications` — create a property plus registration application in one database transaction.
- `PUT /api/applications/{code}/status` — validate a workflow transition, update the application and linked property, and write an audit event in one transaction. Same-status updates are idempotent; invalid transitions return 409.
- `GET /api/audit-events` — recent persisted audit events.

Configure `ConnectionStrings:Registry` through user secrets or `ConnectionStrings__Registry`. Configure `Cors:AllowedOrigins` for the frontend origin.

## Important limitations
Authentication/authorisation is not implemented. Workflow rules are guardrails, not access control. The API must be used only in a controlled development environment and must not be exposed publicly with real data. Document upload/storage, formal audit immutability, rate limiting, OCR, ML, blockchain and government integrations are not implemented. Status decisions are not legally authoritative. Add migrations, validation tests and server-side identity policies before production use.
