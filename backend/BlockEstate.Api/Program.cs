using BlockEstate.Api.Data;
using BlockEstate.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("Registry")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=BlockEstateNexus;Trusted_Connection=True;TrustServerCertificate=True";
builder.Services.AddDbContext<RegistryDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("Frontend");
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    service = "BlockEstate Nexus API",
    environment = app.Environment.EnvironmentName,
    note = "Health response does not imply database or external integrations are ready."
}));

app.MapGet("/api/properties", async (RegistryDbContext db, string? search, string? status, CancellationToken ct) =>
{
    IQueryable<PropertyRecord> query = db.Properties.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(search))
    {
        var term = search.Trim();
        query = query.Where(p => p.PropertyCode.Contains(term) || p.Title.Contains(term) ||
            p.Location.Contains(term) || p.RecordedOwner.Contains(term) || p.ApplicationCode.Contains(term));
    }
    if (!string.IsNullOrWhiteSpace(status)) query = query.Where(p => p.Status == status);
    var rows = await query.OrderByDescending(p => p.UpdatedAt).Take(250).ToListAsync(ct);
    return Results.Ok(rows.Select(ToResponse));
});

app.MapGet("/api/properties/{code}", async Task<IResult> (string code, RegistryDbContext db, CancellationToken ct) =>
{
    var property = await db.Properties.AsNoTracking().SingleOrDefaultAsync(p => p.PropertyCode == code, ct);
    if (property is null) return Results.NotFound(new { message = "Property record was not found." });
    return Results.Ok(ToResponse(property));
});

app.MapPost("/api/properties", async Task<IResult> (CreatePropertyRequest request, RegistryDbContext db, CancellationToken ct) =>
{
    var errors = Validate(request);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var code = request.PropertyCode.Trim();
    if (await db.Properties.AnyAsync(p => p.PropertyCode == code, ct))
        return Results.Conflict(new { message = "Property code already exists." });

    var property = new PropertyRecord
    {
        PropertyCode = code,
        Title = request.Title.Trim(),
        PropertyType = request.PropertyType.Trim(),
        Location = request.Location.Trim(),
        RecordedOwner = request.RecordedOwner.Trim(),
        Area = request.Area.Trim(),
        Status = "Under review",
        RiskLevel = "Review",
        ApplicationCode = string.IsNullOrWhiteSpace(request.ApplicationCode)
            ? $"APP-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
            : request.ApplicationCode.Trim(),
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };
    db.Properties.Add(property);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/properties/{property.PropertyCode}", ToResponse(property));
});

app.MapPut("/api/properties/{code}/status", async Task<IResult> (string code, UpdateStatusRequest request, RegistryDbContext db, CancellationToken ct) =>
{
    if (!new[] { "Under review", "Verified", "Action required" }.Contains(request.Status))
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["status"] = ["Status is not supported."] });
    var property = await db.Properties.SingleOrDefaultAsync(p => p.PropertyCode == code, ct);
    if (property is null) return Results.NotFound(new { message = "Property record was not found." });
    property.Status = request.Status;
    property.UpdatedAt = DateTimeOffset.UtcNow;
    await db.SaveChangesAsync(ct);
    return Results.Ok(ToResponse(property));
});

app.Run();

static object ToResponse(PropertyRecord p) => new
{
    id = p.PropertyCode,
    title = p.Title,
    type = p.PropertyType,
    location = p.Location,
    owner = p.RecordedOwner,
    area = p.Area,
    status = p.Status,
    updated = p.UpdatedAt.ToLocalTime().ToString("dd MMM yyyy, HH:mm"),
    risk = p.RiskLevel,
    applicationId = p.ApplicationCode
};

static Dictionary<string, string[]> Validate(CreatePropertyRequest request)
{
    var errors = new Dictionary<string, string[]>();
    void Require(string key, string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) errors[key] = ["This field is required."];
        else if (value.Trim().Length > max) errors[key] = [$"Must be {max} characters or fewer."];
    }
    Require("propertyCode", request.PropertyCode, 40);
    Require("title", request.Title, 180);
    Require("propertyType", request.PropertyType, 80);
    Require("location", request.Location, 240);
    Require("recordedOwner", request.RecordedOwner, 160);
    Require("area", request.Area, 80);
    if (request.ApplicationCode?.Length > 40) errors["applicationCode"] = ["Must be 40 characters or fewer."];
    return errors;
}

public sealed record CreatePropertyRequest(
    string PropertyCode, string Title, string PropertyType, string Location,
    string RecordedOwner, string Area, string? ApplicationCode);
public sealed record UpdateStatusRequest(string Status);
