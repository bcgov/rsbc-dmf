using OpenQA.Selenium;
using Protractor;
using System;
using System.Threading;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to perform basic tests on the doctors' portal

@pipeline
Scenario: Doctors' Portal Health Check
    When I log in to the doctors' portal
    And the content is displayed for the DMER dashboard
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And the content is displayed for the ICBC tombstone data
    Then I log out of the portal

Scenario: Vision Assessment
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I click on the Visual Assessment tab
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Next button
    And I click on the Next button
    And I click on the Next button
    And the content is displayed for the DMER clean pass
    Then I log out of the portal
*/

namespace bdd_tests
{
    [FeatureFile("./DoctorsPortalHealthCheck.feature")]
    public sealed class DoctorsPortalHealthCheck : TestBase
    {
        [When(@"I log in to the doctors' portal")]
        public void DoctorsPortalLogIn()
        {
            var DoctorsPortalUri = configuration["baseUri"];
            var cardSerialNumber = configuration["csn"];
            var passcode = configuration["passcode"];

            ngDriver.IgnoreSynchronization = true;
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{DoctorsPortalUri}");

            // click on Virtual card testing button
            var virtualCardTestingButton = ngDriver.WrappedDriver.FindElement(By.Id("tile_virtual_device_div_id"));
            virtualCardTestingButton.Click();

            Thread.Sleep(3000);

            // enter Card Serial Number
            var cardSerialNumberInput = ngDriver.WrappedDriver.FindElement(By.Id("csn"));
            cardSerialNumberInput.SendKeys(cardSerialNumber);

            // click on the Continue button
            var continueButton = ngDriver.WrappedDriver.FindElement(By.Id("continue"));
            continueButton.Click();

            // enter Passcode
            var passcodeInput = ngDriver.WrappedDriver.FindElement(By.Id("passcode"));
            passcodeInput.SendKeys(passcode);

            // click on the second Continue button
            var secondContinueButton = ngDriver.WrappedDriver.FindElement(By.Id("btnSubmit"));
            secondContinueButton.Click();
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

            if (contentType == "the ICBC tombstone data")
            {
                Thread.Sleep(3000);

                ngDriver.WrappedDriver.SwitchTo().Frame(0);

                Thread.Sleep(10000);

                NgWebElement driversLicence = null;
                for (var i = 0; i < 60; i++)
                    try
                    {
                        var names = ngDriver.FindElements(By.Id("e9egu0c-textTargetDriverLicense"));
                        if (names.Count > 0)
                        {
                            driversLicence = names[0];
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                Assert.True(driversLicence.GetAttribute("value") == "0200700");

                // confirm value of driver's surname
                var driverSurname = ngDriver.WrappedDriver.FindElement(By.Id("ejdutis-textTargetDriverName"));
                Assert.True(driverSurname.GetAttribute("value") == "PAKKER");

                // confirm value of driver's given name
                var driverGivenName = ngDriver.WrappedDriver.FindElement(By.Id("ewjvhp-textTargetDriverFirstname"));
                Assert.True(driverGivenName.GetAttribute("value") == "PETER");

                // confirm value of driver's date of birth
                var driverDateOfBirth = ngDriver.WrappedDriver.FindElement(By.Id("evc5z8l-tDateTargetDriverBirthdate"));
                Assert.True(driverDateOfBirth.GetAttribute("value") == "1987-03-26");

                // confirm value of driver's gender
                var driverGender = ngDriver.WrappedDriver.FindElement(By.Id("e1iwjqo-male"));
                Assert.True(driverGender.GetAttribute("checked") == "true");

                // confirm value of driver's city
                var driverCity = ngDriver.WrappedDriver.FindElement(By.Id("ewoszlf-textTargetDriverCity"));
                Assert.True(driverCity.GetAttribute("value") == "VICTORIA");

                // confirm value of driver's street address 1
                var driverStreetAddress1 = ngDriver.WrappedDriver.FindElement(By.Id("e94cf9y-textTargetDriverAddr1"));
                Assert.True(driverStreetAddress1.GetAttribute("value") == "129 DEAN RD");

                // confirm value of driver's postal code
                var driverPostalCode = ngDriver.WrappedDriver.FindElement(By.Id("e3d39e-textTargetDriverPostal"));
                Assert.True(driverPostalCode.GetAttribute("value") == "V8K 2K4");
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

            if (element == "the Known Medical Conditions and Histories tab")
            {
                var knownMedicalConditionsAndHistories = ngDriver.FindElement(By.LinkText("Known Medical Conditions and Histories"));
                knownMedicalConditionsAndHistories.Click();
            }

            if (element == "the doctors' portal")
            {
                var DoctorsPortalUri = configuration["baseUri"];

                ngDriver.IgnoreSynchronization = true;
                ngDriver.WrappedDriver.Navigate().GoToUrl($"{DoctorsPortalUri}");
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