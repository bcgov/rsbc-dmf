using OpenQA.Selenium;
using Protractor;
using System;
using System.Threading;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DMERe2e.feature
    As a Driver Medical Fitness SME
    I want to run an end-to-end test for the DMER

@pipeline
Scenario: E2E Test
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 333
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the form submit button
    And I log out of the portal
    And I log in to the Dynamics portal
    And I wait for the Dynamics homepage content to be displayed
    Then I log out of the portal
*/

namespace bdd_tests
{
    [FeatureFile("./DMERe2e.feature")]
    public sealed class DMERe2e : TestBase
    {
    }
}