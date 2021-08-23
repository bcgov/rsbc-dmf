using OpenQA.Selenium;
using Protractor;
using System;
using System.Threading;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: DMERSyncope.feature
    As a Driver Medical Fitness SME
    I want to confirm the syncope business rules for a DMER

Scenario: Syncope Unexplained Single No Repeat
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the single unexplained no repeat syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Unexplained Recurrent Past Year
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the recurrent unexplained past year syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope All Areas of Concern
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the single unexplained no repeat syncope details
    And I enter the all areas of concern that apply to patient's syncope experience
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Currently Untreated Single
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the currently untreated no repeat syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Currently Untreated Recurrent
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the untreated currently recurrent syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Diagnosed, Treated Successfully, Single, Recent
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the diagnosed treated successfully single recent syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Diagnosed, Treated Successfully, Recurrent, Not Recent
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the not recent diagnosed treated successfully recurrent syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Reversible, Treated Successfully, Single
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the reversible, treated successfully single syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Reversible, Treated Successfully, Recurrent
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the treated successfully recurrent syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

    Scenario: Syncope Situational, Avoidable Trigger, Single, Past 7 Days No
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the situational single past 7 days yes syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal

Scenario: Syncope Situational, Avoidable Trigger, Single, Past 7 Days Yes
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 222
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the past 7 days no situational recurrent syncope details
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the Submit Form button
    Then I log out of the portal
*/

namespace bdd_tests
{
    [FeatureFile("./DMERSyncope.feature")]
    public sealed class DMERSyncope : TestBase
    {
    }
}