Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to perform basic tests on the doctors' portal

Scenario: Doctors' Portal Health Check
    When I log in to the doctors' portal
    And the content is displayed for the DMER dashboard
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I click on the Known Medical Conditions and Histories tab
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