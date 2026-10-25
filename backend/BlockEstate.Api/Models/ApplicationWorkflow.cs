namespace BlockEstate.Api.Models;
/// <summary>Workflow guardrails only; this does not authenticate or authorise callers.</summary>
public static class ApplicationWorkflow
{
    public static bool CanTransition(string current, string next) => current switch
    {
        "Submitted" => next is "Under review" or "Action required" or "Rejected",
        "Under review" => next is "Action required" or "Approved" or "Rejected",
        "Action required" => next is "Under review" or "Rejected",
        "Approved" or "Rejected" => false,
        _ => false
    };
}
