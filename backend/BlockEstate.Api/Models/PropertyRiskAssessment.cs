namespace BlockEstate.Api.Models;
public sealed record PropertyRiskFinding(string Code, int Points, string Severity, string Explanation);
public sealed record PropertyRiskAssessment(string RuleSetVersion, string PropertyCode, int Score, string Level, bool RequiresHumanReview, IReadOnlyList<PropertyRiskFinding> Findings, string Disclaimer);
/// <summary>Deterministic, explainable prototype rules—not ML, title search, or a fraud verdict.</summary>
public static class PropertyRiskRules
{
    public const string Version = "rules-v1";
    public static PropertyRiskAssessment Assess(PropertyRecord subject, IReadOnlyCollection<PropertyRecord> properties, IReadOnlyCollection<RegistrationApplication> applications)
    {
        var findings = new List<PropertyRiskFinding>();
        var others = properties.Where(p => p.PropertyCode != subject.PropertyCode).ToArray();
        if (others.Any(p => Same(p.Title, subject.Title) && Same(p.Location, subject.Location)))
            findings.Add(new("DUPLICATE_DESCRIPTION", 40, "Review", "Another record has the same normalized title and location. Compare source records; this may be legitimate and is not proof of fraud."));
        if (others.Any(p => Same(p.ApplicationCode, subject.ApplicationCode)))
            findings.Add(new("DUPLICATE_APPLICATION_REFERENCE", 50, "Elevated", "The application reference is associated with more than one property record. Check source records and data linkage."));
        var app = applications.FirstOrDefault(a => Same(a.ApplicationCode, subject.ApplicationCode) && Same(a.PropertyCode, subject.PropertyCode));
        if (Same(subject.Status, "Verified") && (app is null || !Same(app.Status, "Approved")))
            findings.Add(new("VERIFICATION_WORKFLOW_MISMATCH", 25, "Review", "Property is marked Verified but no matching Approved application was found. Reconcile the workflow."));
        else if (app is not null && Same(app.Status, "Approved") && !Same(subject.Status, "Verified"))
            findings.Add(new("APPROVAL_STATUS_MISMATCH", 20, "Review", "Application is Approved while the property is not marked Verified. Reconcile both records."));
        if (string.IsNullOrWhiteSpace(subject.RecordedOwner))
            findings.Add(new("MISSING_RECORDED_OWNER", 10, "Review", "Recorded owner is empty; confirm through an authorised source."));
        if (string.IsNullOrWhiteSpace(subject.Location))
            findings.Add(new("MISSING_LOCATION", 10, "Review", "Property location is empty, limiting reliable matching."));
        var score = Math.Min(100, findings.Sum(f => f.Points));
        return new(Version, subject.PropertyCode, score, score >= 50 ? "Elevated" : score >= 20 ? "Review" : "Low", score >= 20, findings,
            "Prototype rule checks only. Not an ML prediction, official title search, legal determination, or proof of fraud. Authorised human review is required.");
    }
    private static bool Same(string? a, string? b) => string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
    private static string Normalize(string? value) => string.Join(' ', (value ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
