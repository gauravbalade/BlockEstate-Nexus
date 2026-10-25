using BlockEstate.Api.Models;
using Xunit;
namespace BlockEstate.Api.Tests;
public sealed class PropertyRiskRulesTests
{
 [Fact] public void Duplicate_title_and_location_requires_review(){var a=P("BEN-1","Lakeview A-1","Airoli","APP-1","Under review");var b=P("BEN-2"," lakeview  A-1 ","airoli","APP-2","Under review");var r=PropertyRiskRules.Assess(a,[a,b],[]);Assert.Equal(40,r.Score);Assert.Equal("Review",r.Level);Assert.Contains(r.Findings,f=>f.Code=="DUPLICATE_DESCRIPTION");}
 [Fact] public void Duplicate_application_reference_is_elevated(){var a=P("BEN-1","Plot 1","Panvel","APP-X","Under review");var b=P("BEN-2","Plot 2","Thane","APP-X","Under review");var r=PropertyRiskRules.Assess(a,[a,b],[]);Assert.Equal(50,r.Score);Assert.Equal("Elevated",r.Level);}
 [Fact] public void Verified_without_approved_application_is_flagged(){var a=P("BEN-1","Plot 1","Panvel","APP-1","Verified");var app=new RegistrationApplication{ApplicationCode="APP-1",PropertyCode="BEN-1",Status="Under review"};var r=PropertyRiskRules.Assess(a,[a],[app]);Assert.Contains(r.Findings,f=>f.Code=="VERIFICATION_WORKFLOW_MISMATCH");}
 [Fact] public void Missing_fields_are_explained(){var a=P("BEN-1","Plot 1","","APP-1","Under review");a.RecordedOwner="";var r=PropertyRiskRules.Assess(a,[a],[]);Assert.Contains(r.Findings,f=>f.Code=="MISSING_LOCATION");Assert.Contains(r.Findings,f=>f.Code=="MISSING_RECORDED_OWNER");}
 private static PropertyRecord P(string c,string title,string location,string app,string status)=>new(){PropertyCode=c,Title=title,Location=location,ApplicationCode=app,Status=status,RecordedOwner="Sample Owner",Area="100 sq ft"};
}
