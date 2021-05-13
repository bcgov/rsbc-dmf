using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to confirm that I can view the doctors' portal

Scenario: Doctors' Portal Health Check
    When I click on the doctors' portal
    Then the portal is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./DoctorsPortalHealthCheck.feature")]
    [Collection("Liquor")]
    public sealed class DoctorsPortalHealthCheck : TestBase
    {
        [When(@"I click on the doctors' portal")]
        public void DoctorsPortalHealthCheckClick()
        {
            ngDriver.Navigate().GoToUrl($"http://dev1-dft-doctors.silver.devops.bcgov/");
            ngDriver.WaitForAngular();
        }


        [Then(@"the portal is displayed")]
        public void DoctorsPortalDisplayed()
        {
            ngDriver.WrappedDriver.PageSource.Contains("Welcome to the doctor's portal!");
        }
    }
}