using BlockEstate.Api.Data;
using BlockEstate.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("Registry")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=BlockEstateNexus;Trusted_Connection=True;TrustServerCertificate=True";
builder.Services.AddDbContext<RegistryDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
    p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("Frontend");

// This scaffold has no authentication/authorisation yet. Fail closed for writes
// outside Development so an accidentally exposed deployment cannot mutate records.
app.Use(async (context, next) =>
{
    var isWrite = HttpMethods.IsPost(context.Request.Method)
        || HttpMethods.IsPut(context.Request.Method)
        || HttpMethods.IsPatch(context.Request.Method)
        || HttpMethods.IsDelete(context.Request.Method);
    if (!app.Environment.IsDevelopment() && isWrite)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Write operations are disabled outside Development until authentication and authorisation are implemented."
        });
        return;
    }
    await next();
});
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "BlockEstate Nexus API", note = "Liveness only; this response does not confirm database readiness." }));
app.MapGet("/api/health/ready", async (RegistryDbContext db, CancellationToken ct) =>
{
    try
    {
        var connected = await db.Database.CanConnectAsync(ct);
        return connected
            ? Results.Ok(new { status = "ready", database = "reachable" })
            : Results.Json(new { status = "not_ready", database = "unreachable" }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception)
    {
        // Keep connection details and infrastructure secrets out of the health response.
        return Results.Json(new { status = "not_ready", database = "unreachable" }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapGet("/api/properties", async (RegistryDbContext db, string? search, string? status, CancellationToken ct) =>
{
    IQueryable<PropertyRecord> q = db.Properties.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(search)) {
        var term = search.Trim();
        q = q.Where(p => p.PropertyCode.Contains(term) || p.Title.Contains(term) || p.Location.Contains(term) || p.RecordedOwner.Contains(term) || p.ApplicationCode.Contains(term));
    }
    if (!string.IsNullOrWhiteSpace(status)) q = q.Where(p => p.Status == status);
    return Results.Ok((await q.OrderByDescending(p => p.UpdatedAt).Take(250).ToListAsync(ct)).Select(ToProperty));
});
app.MapGet("/api/properties/{code}/risk-assessment", async Task<IResult> (string code, RegistryDbContext db, CancellationToken ct) =>
{
    var subject = await db.Properties.AsNoTracking().SingleOrDefaultAsync(p => p.PropertyCode == code, ct);
    if (subject is null) return Results.NotFound(new { message = "Property record was not found." });
    var properties = await db.Properties.AsNoTracking().OrderBy(p => p.PropertyCode).Take(5000).ToListAsync(ct);
    var applications = await db.Applications.AsNoTracking().OrderBy(a => a.ApplicationCode).Take(5000).ToListAsync(ct);
    return Results.Ok(PropertyRiskRules.Assess(subject, properties, applications));
});

app.MapGet("/api/properties/{code}", async Task<IResult> (string code, RegistryDbContext db, CancellationToken ct) =>
{
    var p = await db.Properties.AsNoTracking().SingleOrDefaultAsync(x => x.PropertyCode == code, ct);
    if (p is null) return Results.NotFound(new { message = "Property record was not found." });
    return Results.Ok(ToProperty(p));
});
app.MapPost("/api/properties", async Task<IResult> (CreatePropertyRequest r, RegistryDbContext db, CancellationToken ct) =>
{
    var errors = Validate(r.Title, r.PropertyType, r.Location, r.RecordedOwner, r.Area, r.PropertyCode);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var code = r.PropertyCode.Trim();
    if (await db.Properties.AnyAsync(p => p.PropertyCode == code, ct)) return Results.Conflict(new { message = "Property code already exists." });
    var p = new PropertyRecord { PropertyCode=code, Title=r.Title.Trim(), PropertyType=r.PropertyType.Trim(), Location=r.Location.Trim(), RecordedOwner=r.RecordedOwner.Trim(), Area=r.Area.Trim(), Status="Under review", RiskLevel="Review", ApplicationCode=string.IsNullOrWhiteSpace(r.ApplicationCode)?NewAppCode():r.ApplicationCode.Trim() };
    db.Properties.Add(p);
    db.AuditEvents.Add(Audit("Property record created",code,"Registration","Created through property API."));
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/properties/{code}",ToProperty(p));
});

app.MapGet("/api/applications", async (RegistryDbContext db, string? status, CancellationToken ct) =>
{
    var query = from a in db.Applications.AsNoTracking()
                join p in db.Properties.AsNoTracking() on a.PropertyCode equals p.PropertyCode
                select new { a, p.Title };
    if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.a.Status == status);
    var rows = await query.OrderByDescending(x => x.a.UpdatedAt).Take(250).ToListAsync(ct);
    return Results.Ok(rows.Select(x => ToApplication(x.a, x.Title)));
});
app.MapGet("/api/applications/{code}", async Task<IResult> (string code, RegistryDbContext db, CancellationToken ct) =>
{
    var row = await (from a in db.Applications.AsNoTracking()
                     join p in db.Properties.AsNoTracking() on a.PropertyCode equals p.PropertyCode
                     where a.ApplicationCode == code
                     select new { Application = a, p.Title }).SingleOrDefaultAsync(ct);
    return row is null
        ? Results.NotFound(new { message = "Application was not found." })
        : Results.Ok(ToApplication(row.Application, row.Title));
});
app.MapPost("/api/applications", async Task<IResult> (CreateApplicationRequest r, RegistryDbContext db, CancellationToken ct) =>
{
    var errors = Validate(r.Title,r.PropertyType,r.Location,r.RecordedOwner,r.Area,null);
    if (string.IsNullOrWhiteSpace(r.ApplicantName) || r.ApplicantName.Trim().Length > 160) errors["applicantName"]=["Applicant name is required and must be 160 characters or fewer."];
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var appCode=NewAppCode(); var propertyCode=NewPropertyCode();
    while(await db.Properties.AnyAsync(p=>p.PropertyCode==propertyCode,ct)) propertyCode=NewPropertyCode();
    var now=DateTimeOffset.UtcNow;
    var p=new PropertyRecord { PropertyCode=propertyCode,Title=r.Title.Trim(),PropertyType=r.PropertyType.Trim(),Location=r.Location.Trim(),RecordedOwner=r.RecordedOwner.Trim(),Area=r.Area.Trim(),Status="Under review",RiskLevel="Review",ApplicationCode=appCode,CreatedAt=now,UpdatedAt=now };
    var a=new RegistrationApplication { ApplicationCode=appCode,PropertyCode=propertyCode,ApplicantName=r.ApplicantName.Trim(),WorkflowType="New registration",Status="Submitted",DocumentsListed=0,Priority="Normal",CreatedAt=now,UpdatedAt=now };
    await using var tx=await db.Database.BeginTransactionAsync(ct);
    db.Properties.Add(p); db.Applications.Add(a);
    db.AuditEvents.Add(Audit("Registration application submitted",appCode,"Registration",$"Property reference {propertyCode}; pending authorised review."));
    await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    return Results.Created($"/api/applications/{appCode}",new { property=ToProperty(p), application=ToApplication(a,p.Title) });
});
app.MapPut("/api/applications/{code}/status", async Task<IResult> (string code, UpdateApplicationStatusRequest r, RegistryDbContext db, CancellationToken ct) =>
{
    if (!new[]{"Submitted","Under review","Action required","Approved","Rejected"}.Contains(r.Status))
        return Results.ValidationProblem(new Dictionary<string,string[]>{{"status",["Status is not supported."]}});
    var a=await db.Applications.SingleOrDefaultAsync(x=>x.ApplicationCode==code,ct);
    if(a is null) return Results.NotFound(new {message="Application was not found."});
    var old=a.Status;
    if (old != r.Status && !ApplicationWorkflow.CanTransition(old, r.Status))
        return Results.Conflict(new { message = "Status transition from '" + old + "' to '" + r.Status + "' is not allowed.", currentStatus = old });
    if (old == r.Status) return Results.Ok(ToApplication(a, await db.Properties.AsNoTracking().Where(x => x.PropertyCode == a.PropertyCode).Select(x => x.Title).SingleOrDefaultAsync(ct) ?? a.PropertyCode));
    await using var tx = await db.Database.BeginTransactionAsync(ct);
    a.Status=r.Status; a.UpdatedAt=DateTimeOffset.UtcNow;
    var p=await db.Properties.SingleOrDefaultAsync(x=>x.PropertyCode==a.PropertyCode,ct);
    if(p is not null){p.Status=r.Status=="Approved"?"Verified":r.Status=="Rejected"?"Action required":r.Status=="Action required"?"Action required":"Under review";p.UpdatedAt=DateTimeOffset.UtcNow;}
    db.AuditEvents.Add(Audit("Application status changed: " + old + " → " + r.Status,code,"Review","API workflow event. Authentication is not implemented in this scaffold."));
    await db.SaveChangesAsync(ct);
    await tx.CommitAsync(ct);
    return Results.Ok(ToApplication(a,p?.Title ?? a.PropertyCode));
});
app.MapGet("/api/audit-events", async (RegistryDbContext db,CancellationToken ct) =>
{
    var rows=await db.AuditEvents.AsNoTracking().OrderByDescending(e=>e.CreatedAt).Take(200).ToListAsync(ct);
    return Results.Ok(rows.Select(e=>new {time=e.CreatedAt.ToLocalTime().ToString("HH:mm:ss"),actor=e.ActorLabel,action=e.Action,target=e.Target,kind=e.Kind}));
});
app.Run();

static object ToProperty(PropertyRecord p)=>new{id=p.PropertyCode,title=p.Title,type=p.PropertyType,location=p.Location,owner=p.RecordedOwner,area=p.Area,status=p.Status,updated=p.UpdatedAt.ToLocalTime().ToString("dd MMM yyyy, HH:mm"),risk=p.RiskLevel,applicationId=p.ApplicationCode};
static object ToApplication(RegistrationApplication a,string title)=>new{id=a.ApplicationCode,property=title,applicant=a.ApplicantName,kind=a.WorkflowType,status=a.Status,documents=$"{a.DocumentsListed} / 5",priority=a.Priority};
static AuditEvent Audit(string action,string target,string kind,string details)=>new(){ActorLabel="API workflow (unauthenticated scaffold)",Action=action,Target=target,Kind=kind,Details=details,CreatedAt=DateTimeOffset.UtcNow};
static string NewAppCode()=>$"APP-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
static string NewPropertyCode()=>$"BEN-MH-{Random.Shared.Next(10000,99999)}";
static Dictionary<string,string[]> Validate(string? title,string? type,string? location,string? owner,string? area,string? code)
{
 var errors=new Dictionary<string,string[]>();
 void Req(string key,string? value,int max){if(string.IsNullOrWhiteSpace(value))errors[key]=["This field is required."];else if(value.Trim().Length>max)errors[key]=[$"Must be {max} characters or fewer."];}
 Req("title",title,180);Req("propertyType",type,80);Req("location",location,240);Req("recordedOwner",owner,160);Req("area",area,80);
 if(code is not null)Req("propertyCode",code,40);return errors;
}
public sealed record CreatePropertyRequest(string PropertyCode,string Title,string PropertyType,string Location,string RecordedOwner,string Area,string? ApplicationCode);
public sealed record CreateApplicationRequest(string Title,string PropertyType,string Location,string RecordedOwner,string Area,string ApplicantName);
public sealed record UpdateApplicationStatusRequest(string Status);
