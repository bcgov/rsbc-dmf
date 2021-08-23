using OpenQA.Selenium;
using Protractor;
using System;
using System.Threading;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to perform a health check on the doctors' portal

@pipeline
Scenario: Doctors' Portal Tombstone Check
    When I log in to the doctors' portal
    And the content is displayed for the DMER dashboard
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And the content is displayed for the ICBC tombstone data
    Then I log out of the portal

@pipeline
Scenario: Doctors' Portal Provider Check
    When I log in to the doctors' portal
    And the content is displayed for the DMER dashboard
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And the content is displayed for the provider
    Then I log out of the portal
*/

namespace bdd_tests
{
    [FeatureFile("./DoctorsPortalHealthCheck.feature")]
    public sealed class DoctorsPortalHealthCheck : TestBase
    {
    }
}