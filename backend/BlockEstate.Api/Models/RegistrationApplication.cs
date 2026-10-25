namespace BlockEstate.Api.Models;

public sealed class RegistrationApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ApplicationCode { get; set; } = string.Empty;
    public string PropertyCode { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string WorkflowType { get; set; } = "New registration";
    public string Status { get; set; } = "Submitted";
    public int DocumentsListed { get; set; }
    public string Priority { get; set; } = "Normal";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
