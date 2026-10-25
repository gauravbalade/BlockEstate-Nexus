using BlockEstate.Api.Models;
using Xunit;
namespace BlockEstate.Api.Tests;
public sealed class ApplicationWorkflowTests
{
 [Theory]
 [InlineData("Submitted","Under review")][InlineData("Submitted","Action required")][InlineData("Submitted","Rejected")]
 [InlineData("Under review","Action required")][InlineData("Under review","Approved")][InlineData("Under review","Rejected")]
 [InlineData("Action required","Under review")][InlineData("Action required","Rejected")]
 public void Allows_defined_transitions(string current,string next)=>Assert.True(ApplicationWorkflow.CanTransition(current,next));
 [Theory]
 [InlineData("Submitted","Approved")][InlineData("Under review","Submitted")][InlineData("Action required","Approved")]
 [InlineData("Approved","Under review")][InlineData("Approved","Rejected")][InlineData("Rejected","Under review")][InlineData("Unknown","Approved")]
 public void Rejects_unsupported_transitions(string current,string next)=>Assert.False(ApplicationWorkflow.CanTransition(current,next));
}
