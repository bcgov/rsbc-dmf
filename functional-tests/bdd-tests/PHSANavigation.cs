using Xunit;
using Xunit.Gherkin.Quick;
using System;
using System.Threading;
using OpenQA.Selenium;

/*
Feature: PHSANavigation
    As a medical professional
    I want to confirm that I can view the DMER form on the PHSA site

Scenario: PHSA Navigation
    When I click on the PHSA link
    And I enter the PHSA credentials
    And I click on the authorization button
    And I click on the DMER
    Then I click on the Submit button
*/

namespace bdd_tests
{
    [FeatureFile("./*.feature")]
    public sealed class PHSANavigation : TestBase
    {
        [When(@"I click on the PHSA link")]
        public void PHSALinkClick()
        {
            var baseUri = configuration["PHSAUri"];
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}");
        }


        [And(@"I enter the PHSA credentials")]
        public void EnterCredentials()
        {
            var tenant = configuration["tenant"];
            var email = configuration["email"];
            var password = configuration["password"];

            // enter the tenant
            ngDriver.WrappedDriver.FindElement(By.Id("e1r1s3d-tenant")).SendKeys(tenant);

            // enter the email
            ngDriver.WrappedDriver.FindElement(By.Id("edmb8b-email")).SendKeys(email);

            // enter the password
            ngDriver.WrappedDriver.FindElement(By.Id("elox36-password")).SendKeys(password);
        }


        [And(@"I click on the authorization button")]
        public void AuthorizationButton()
        {
            ngDriver.WrappedDriver.FindElement(By.XPath("//*[@id='eepe4gn']/button")).Click();
        }


        [And(@"I click on the DMER")]
        public void SampleForm()
        {
        }


        [And(@"I click on the calendar")]
        public void SampleElement()
        {
        }


        [Then(@"I click on the Submit button")]
        public void SubmitButton()
        {
        }
    }
}