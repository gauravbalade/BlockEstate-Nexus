/* BlockEstate Nexus SQL Server schema reference. Apply to a development DB only. */
CREATE TABLE dbo.PropertyRecords (
 Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_PropertyRecords PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
 PropertyCode NVARCHAR(40) NOT NULL, Title NVARCHAR(180) NOT NULL, PropertyType NVARCHAR(80) NOT NULL,
 Location NVARCHAR(240) NOT NULL, RecordedOwner NVARCHAR(160) NOT NULL, Area NVARCHAR(80) NOT NULL,
 Status NVARCHAR(40) NOT NULL CONSTRAINT DF_PropertyRecords_Status DEFAULT N'Under review',
 RiskLevel NVARCHAR(30) NOT NULL CONSTRAINT DF_PropertyRecords_Risk DEFAULT N'Review',
 ApplicationCode NVARCHAR(40) NOT NULL, CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 CONSTRAINT UQ_PropertyRecords_PropertyCode UNIQUE(PropertyCode)
);
CREATE INDEX IX_PropertyRecords_ApplicationCode ON dbo.PropertyRecords(ApplicationCode);
CREATE INDEX IX_PropertyRecords_Status_UpdatedAt ON dbo.PropertyRecords(Status,UpdatedAt DESC);

CREATE TABLE dbo.RegistrationApplications (
 Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_RegistrationApplications PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
 ApplicationCode NVARCHAR(40) NOT NULL, PropertyCode NVARCHAR(40) NOT NULL,
 ApplicantName NVARCHAR(160) NOT NULL, WorkflowType NVARCHAR(80) NOT NULL,
 Status NVARCHAR(40) NOT NULL CONSTRAINT DF_RegistrationApplications_Status DEFAULT N'Submitted',
 DocumentsListed INT NOT NULL CONSTRAINT DF_RegistrationApplications_Documents DEFAULT 0,
 Priority NVARCHAR(30) NOT NULL CONSTRAINT DF_RegistrationApplications_Priority DEFAULT N'Normal',
 CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 CONSTRAINT UQ_RegistrationApplications_ApplicationCode UNIQUE(ApplicationCode),
 CONSTRAINT FK_RegistrationApplications_PropertyCode FOREIGN KEY(PropertyCode) REFERENCES dbo.PropertyRecords(PropertyCode)
);
CREATE INDEX IX_RegistrationApplications_Status ON dbo.RegistrationApplications(Status);

CREATE TABLE dbo.AuditEvents (
 Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_AuditEvents PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
 ActorLabel NVARCHAR(160) NOT NULL, Action NVARCHAR(240) NOT NULL, Target NVARCHAR(80) NOT NULL,
 Kind NVARCHAR(50) NOT NULL, Details NVARCHAR(1000) NOT NULL DEFAULT N'',
 CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
CREATE INDEX IX_AuditEvents_CreatedAt ON dbo.AuditEvents(CreatedAt DESC);

/* This initial schema is not a complete production security model. Add auth, ownership/role policies,
   retention rules, protected document metadata/storage and reviewed migrations before real use. */
