/* BlockEstate Nexus initial SQL Server schema reference. Review before applying. */
CREATE TABLE dbo.PropertyRecords (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_PropertyRecords PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PropertyCode NVARCHAR(40) NOT NULL,
    Title NVARCHAR(180) NOT NULL,
    PropertyType NVARCHAR(80) NOT NULL,
    Location NVARCHAR(240) NOT NULL,
    RecordedOwner NVARCHAR(160) NOT NULL,
    Area NVARCHAR(80) NOT NULL,
    Status NVARCHAR(40) NOT NULL CONSTRAINT DF_PropertyRecords_Status DEFAULT N'Under review',
    RiskLevel NVARCHAR(30) NOT NULL CONSTRAINT DF_PropertyRecords_Risk DEFAULT N'Review',
    ApplicationCode NVARCHAR(40) NOT NULL,
    CreatedAt DATETIMEOFFSET NOT NULL CONSTRAINT DF_PropertyRecords_CreatedAt DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL CONSTRAINT DF_PropertyRecords_UpdatedAt DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT UQ_PropertyRecords_PropertyCode UNIQUE (PropertyCode)
);
CREATE INDEX IX_PropertyRecords_ApplicationCode ON dbo.PropertyRecords(ApplicationCode);
CREATE INDEX IX_PropertyRecords_Status_UpdatedAt ON dbo.PropertyRecords(Status, UpdatedAt DESC);

/* Future tables: RegistrationApplications, OwnershipInterests, DocumentMetadata, ReviewFindings,
   AuditEvents, Users/Roles and LedgerAnchors. Use migrations and reviewed foreign keys. */
