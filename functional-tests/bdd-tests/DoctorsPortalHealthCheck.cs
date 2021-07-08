using OpenQA.Selenium;
using Protractor;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to confirm that I can view the doctors' portal

@pipeline
Scenario: Doctors' Portal Health Check
    When I click on the doctors' portal
    And the portal is displayed
    And I enter the login credentials
    And I click on the Submit button
    Then the DMER dashboard is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./DoctorsPortalHealthCheck.feature")]
    public sealed class DoctorsPortalHealthCheck : TestBase
    {
        [When(@"I click on the doctors' portal")]
        public void DoctorsPortalHealthCheckClick()
        {
            var DoctorsPortalUri = configuration["baseUri"];
            
            ngDriver.IgnoreSynchronization = true;
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{DoctorsPortalUri}");
            //ngDriver.WaitForAngular();

            var advancedButton = ngDriver.WrappedDriver.FindElement(By.Id("details-button"));
            advancedButton.Click();

            var proceedLink = ngDriver.WrappedDriver.FindElement(By.Id("proceed-link"));
            proceedLink.Click();
        }


        [And(@"the portal is displayed")]
        public void DoctorsPortalDisplayed()
        {
            ngDriver.WrappedDriver.PageSource.Contains("Welcome to the doctor's portal!");
        }


        [And(@"I enter the login credentials")]
        public void LoginCredentials()
        {
        }


        [And(@"I click on the Submit button")]
        public void SubmitButton()
        {
            var submitButton = ngDriver.WrappedDriver.FindElement(By.CssSelector("button.mat-primary"));
            submitButton.Click();
        }


        [Then(@"the DMER dashboard is displayed")]
        public void DashboardDisplayed()
        {
            ngDriver.WrappedDriver.PageSource.Contains("Dashboard");

            ngDriver.WrappedDriver.PageSource.Contains("Search DMER Case");

            ngDriver.WrappedDriver.PageSource.Contains("Submitted DMER Forms");
        }


        [And(@"I click on the DMER Forms tab")]
        public void DMERFormsTab()
        {
            var DMERFormsTab = ngDriver.FindElement(By.LinkText("DMER Forms"));
            DMERFormsTab.Click();
        }


        [And(@"I click on the Case ID for 111")]
        public void CaseID()
        {
            var caseID = ngDriver.FindElement(By.LinkText("111"));
            caseID.Click();
        }


        [And(@"I refresh the page")]
        public void pageRefresh()
        {
            ngDriver.Navigate().Refresh();
        }


        [And(@"I select the RoadSafetyBC environment")]
        public void selectRSBC()
        {
            var selectRSBC = ngDriver.FindElement(By.XPath("/html/body/div[1]/div/header/nav/div/div/div[1]/label/select/option[10]"));
            selectRSBC.Click();
        }


        [And(@"I select the Testing Resources Quality Assurance form")]
        public void testingForm()
        {
            var selectForm = ngDriver.FindElement(By.XPath("/html/body/div[1]/div/header/nav/div/div/div[2]/label/select/option[4]"));
            selectForm.Click();
        }


        [And(@"I click on the Visual Assessment tab")]
        public void visualAssessmentTab()
        {
            var visualAssessment = ngDriver.FindElement(By.LinkText("Visual Assessment"));
            visualAssessment.Click();
        }

        
        [And(@"I enter the Uncorrected Binocular Vision as 20")]
        public void uncorrectedBinocularVision()
        {

        }


        [And(@"I click on the Next button")]
        public void nextButton()
        {

        }


        [Then(@"this message is displayed: PASS! No Clean Pass responses failed.")]
        public void cleanPassMessage()
        {

        }
    }
}