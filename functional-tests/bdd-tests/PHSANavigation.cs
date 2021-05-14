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
    And I click on Test Elizabeth's ID
    And I click on the calendar
    Then I click on the Submit button
*/

namespace bdd_tests
{
    [FeatureFile("./PHSANavigation.feature")]
    public sealed class PHSANavigation : TestBase
    {
        [When(@"I click on the PHSA link")]
        public void PHSALinkClick()
        {
            ngDriver.Navigate().GoToUrl($"https://apps.form.io/phsa/#/grid/form");
            ngDriver.WaitForAngular();
        }


        [And(@"I click on Test Elizabeth's ID")]
        public void SampleForm()
        {
            ngDriver.Navigate().GoToUrl($"https://apps.form.io/phsa/#/grid/form/609db688d00b15327e93dc7c");
            ngDriver.WaitForAngular();
        }


        [And(@"I click on the calendar")]
        public void SampleElement()
        {
            ngDriver.WrappedDriver.FindElement(By.Id("ew9my2j-textFieldWithCalendarWidget")).Click();
        }


        [Then(@"I click on the Submit button")]
        public void SubmitButton()
        {
            ngDriver.WrappedDriver.FindElement(By.Id("esoswoa")).Click();
        }
    }
}