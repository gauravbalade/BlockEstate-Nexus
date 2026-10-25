# Architecture and trust boundaries

Browser UI → typed frontend API client → ASP.NET Core API → EF Core → SQL Server.

The frontend uses fictional local demo data when no API URL is configured. This is a development convenience, not server persistence or authentication.

## Core domains
Property records; registration applications; ownership interests and transfers; protected document metadata; verification requests and review findings; audit events; explainable risk indicators.

## Trust boundaries
- Treat deeds, identity documents and tax records as sensitive legal/personal data.
- Store document bytes in protected storage, not a public blockchain.
- If a permissioned ledger is added, anchor only minimal integrity digests/references; do not put personal data or document contents on-chain.
- A QR code or risk score is not independent legal proof.
- Enforce authorisation server-side; hiding UI controls is not security.
- Use HTTPS, least-privilege database credentials, validation, audit retention and secret storage before deployment.

## Current implementation
The API currently exposes a health response, property list/search and single-record lookup, property creation, application listing/submission, application status updates, and persisted audit-event listing. SQL Server is the intended persistent store; schema and fictional seed scripts are provided. Authentication, protected document storage, formal workflow state-machine enforcement, OCR/ML, ledger anchoring, notification delivery and government integrations remain future milestones. CI validates frontend and backend compilation; this does not establish database runtime or production readiness.
