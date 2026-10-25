namespace BlockEstate.Api.Models;

public sealed class PropertyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PropertyCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string RecordedOwner { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Status { get; set; } = "Under review";
    public string RiskLevel { get; set; } = "Review";
    public string ApplicationCode { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
