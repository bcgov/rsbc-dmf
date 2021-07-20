using OpenQA.Selenium;
using Protractor;
using System;
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
        }


        /* Temp workaround for S3DMFT-24 - to be removed
         */
        [And(@"I accept the cert request")]
        public void CertRequest()
        {
            var advancedButton = ngDriver.WrappedDriver.FindElement(By.Id("details-button"));
            advancedButton.Click();

            var proceedLink = ngDriver.WrappedDriver.FindElement(By.Id("proceed-link"));
            proceedLink.Click();
        }


        [And(@"the content is displayed for (.*)")]
        public void ContentDisplayed(string contentType)
        {
            if (contentType == "the doctors portal")
            {
                ngDriver.WrappedDriver.PageSource.Contains("Welcome to the doctor's portal!");
            }

            if (contentType == "the DMER dashboard")
            {
                ngDriver.WrappedDriver.PageSource.Contains("Dashboard");

                ngDriver.WrappedDriver.PageSource.Contains("Search DMER Case");

                ngDriver.WrappedDriver.PageSource.Contains("Submitted DMER Forms");
            }

            if (contentType == "the DMER clean pass")
            {
                ngDriver.WrappedDriver.PageSource.Contains("PASS! No Clean Pass responses failed.");
            }
        }


        [And(@"I enter the login credentials")]
        public void LoginCredentials()
        {
        }


        [And(@"I click on (.*)")]
        public void ClickOnElement(string element)
        {
            if (element == "the Submit button")
            {
                NgWebElement submitButton = null;
                for (var i = 0; i < 20; i++)
                    try
                    {
                        // Submit button
                        var names = ngDriver.FindElements(By.CssSelector("button.mat-primary"));
                        if (names.Count > 0)
                        {
                            submitButton = names[0];
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                submitButton.Click();
            }

            if (element == "the DMER Forms tab")
            {
                var DMERFormsTab = ngDriver.FindElement(By.LinkText("DMER Forms"));
                DMERFormsTab.Click();
            }

            if (element == "the Case ID for 111")
            {
                var caseID = ngDriver.FindElement(By.LinkText("111"));
                caseID.Click();
            }
        }


        [And(@"I refresh the page")]
        public void PageRefresh()
        {
            ngDriver.Navigate().Refresh();
        }


        [And(@"I click on the Visual Assessment tab")]
        public void VisualAssessmentTab()
        {
            var visualAssessment = ngDriver.FindElement(By.LinkText("Visual Assessment"));
            visualAssessment.Click();
        }

        
        [And(@"I enter the Uncorrected Binocular Vision as 20")]
        public void UncorrectedBinocularVision()
        {

        }


        [And(@"I click on the Next button")]
        public void NextButton()
        {

        }


        [Then(@"I log out of the portal")]
        public void PortalLogOut()
        {

        }
    }
}