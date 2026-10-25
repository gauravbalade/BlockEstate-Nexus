/* Fictional demo rows for local presentation only. Run after schema.sql against a development database. */
DECLARE @Now DATETIMEOFFSET = SYSDATETIMEOFFSET();

IF NOT EXISTS (SELECT 1 FROM dbo.PropertyRecords WHERE PropertyCode=N'BEN-MH-10482')
 INSERT dbo.PropertyRecords(PropertyCode,Title,PropertyType,Location,RecordedOwner,Area,Status,RiskLevel,ApplicationCode,CreatedAt,UpdatedAt)
 VALUES(N'BEN-MH-10482',N'Lakeview Residency · A-1204',N'Apartment',N'Airoli, Navi Mumbai',N'Ananya Mehta',N'1,180 sq ft',N'Verified',N'Low',N'APP-2026-1842',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.PropertyRecords WHERE PropertyCode=N'BEN-MH-10479')
 INSERT dbo.PropertyRecords(PropertyCode,Title,PropertyType,Location,RecordedOwner,Area,Status,RiskLevel,ApplicationCode,CreatedAt,UpdatedAt)
 VALUES(N'BEN-MH-10479',N'Greenfield Survey Plot 18',N'Residential plot',N'Panvel, Navi Mumbai',N'Rohan Deshmukh',N'2,400 sq ft',N'Under review',N'Review',N'APP-2026-1838',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.PropertyRecords WHERE PropertyCode=N'BEN-MH-10461')
 INSERT dbo.PropertyRecords(PropertyCode,Title,PropertyType,Location,RecordedOwner,Area,Status,RiskLevel,ApplicationCode,CreatedAt,UpdatedAt)
 VALUES(N'BEN-MH-10461',N'Sunrise Industrial Unit 06',N'Industrial',N'Taloja MIDC, Navi Mumbai',N'Kavya Shah',N'4,850 sq ft',N'Action required',N'Elevated',N'APP-2026-1819',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.PropertyRecords WHERE PropertyCode=N'BEN-MH-10433')
 INSERT dbo.PropertyRecords(PropertyCode,Title,PropertyType,Location,RecordedOwner,Area,Status,RiskLevel,ApplicationCode,CreatedAt,UpdatedAt)
 VALUES(N'BEN-MH-10433',N'Orchard Agricultural Parcel',N'Agricultural land',N'Karjat, Raigad',N'Vikram Patil',N'0.8 hectare',N'Verified',N'Low',N'APP-2026-1770',@Now,@Now);

IF NOT EXISTS (SELECT 1 FROM dbo.RegistrationApplications WHERE ApplicationCode=N'APP-2026-1842')
 INSERT dbo.RegistrationApplications(ApplicationCode,PropertyCode,ApplicantName,WorkflowType,Status,DocumentsListed,Priority,CreatedAt,UpdatedAt)
 VALUES(N'APP-2026-1842',N'BEN-MH-10482',N'Ananya Mehta',N'Ownership verification',N'Approved',5,N'Normal',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.RegistrationApplications WHERE ApplicationCode=N'APP-2026-1838')
 INSERT dbo.RegistrationApplications(ApplicationCode,PropertyCode,ApplicantName,WorkflowType,Status,DocumentsListed,Priority,CreatedAt,UpdatedAt)
 VALUES(N'APP-2026-1838',N'BEN-MH-10479',N'Rohan Deshmukh',N'New registration',N'Under review',4,N'Normal',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.RegistrationApplications WHERE ApplicationCode=N'APP-2026-1819')
 INSERT dbo.RegistrationApplications(ApplicationCode,PropertyCode,ApplicantName,WorkflowType,Status,DocumentsListed,Priority,CreatedAt,UpdatedAt)
 VALUES(N'APP-2026-1819',N'BEN-MH-10461',N'Kavya Shah',N'Ownership transfer',N'Action required',3,N'Elevated',@Now,@Now);
IF NOT EXISTS (SELECT 1 FROM dbo.RegistrationApplications WHERE ApplicationCode=N'APP-2026-1770')
 INSERT dbo.RegistrationApplications(ApplicationCode,PropertyCode,ApplicantName,WorkflowType,Status,DocumentsListed,Priority,CreatedAt,UpdatedAt)
 VALUES(N'APP-2026-1770',N'BEN-MH-10433',N'Vikram Patil',N'Ownership verification',N'Approved',5,N'Normal',@Now,@Now);

IF NOT EXISTS (SELECT 1 FROM dbo.AuditEvents WHERE Target=N'APP-2026-1842' AND Action=N'Demo seed records loaded')
 INSERT dbo.AuditEvents(ActorLabel,Action,Target,Kind,Details,CreatedAt)
 VALUES(N'Demo seed script',N'Demo seed records loaded',N'APP-2026-1842',N'Setup',N'Fictional demonstration records. Not official property data.',@Now);
