# Delivery roadmap

## Foundation
- [x] Repository structure and truthful implementation boundaries.
- [x] Responsive interactive frontend prototype.
- [x] ASP.NET Core + EF Core SQL Server API scaffold.
- [x] Schema reference, fictional seed data and architecture notes.
- [x] Frontend production build and backend compile verified in GitHub Actions.
- [x] Add a database-readiness endpoint that returns 503 when SQL Server is unreachable.
- [ ] Run API against a configured SQL Server instance and verify all routes end-to-end.

## Persistence and workflow
- [x] Add registration application and audit-event entities to the API scaffold.
- [ ] Add document metadata, ownership history and review-finding entities.
- [x] Add a guarded application status-transition map and transactional status/audit/property updates.
- [ ] Add automated server-side validation and transition tests.
- [x] Connect property list, registration submission, review status and audit display to API client methods.
- [ ] Add integration tests and verify against a configured SQL Server instance.

## Identity and documents
- [ ] Secure authentication and server-side role policies.
- [ ] Protected document storage, file validation and malware scanning.
- [ ] OCR extraction with confidence and source references.

## Review intelligence and integrity
- [ ] Explainable baseline risk rules with evidence and human review.
- [ ] Evaluate ML only with lawful, representative data and meaningful metrics.
- [ ] Permissioned ledger proof-of-integrity prototype with off-chain documents.
- [ ] QR verification view with explicit scope and freshness.

## Quality and presentation
- [ ] Accessibility, responsive interaction, automated API validation and security tests.
- [x] Add CSV export for filtered registry records and local audit events.
- [ ] Genuine application screenshots and documented test results.
- [x] Editable project presentation draft created.
- [ ] Review the framework version and stack against the college guide's approved proposal.
- [ ] Document limitations, demo setup and viva notes.
