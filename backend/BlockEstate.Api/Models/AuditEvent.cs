namespace BlockEstate.Api.Models;

public sealed class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ActorLabel { get; set; } = "System";
    public string Action { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Kind { get; set; } = "Workflow";
    public string Details { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
