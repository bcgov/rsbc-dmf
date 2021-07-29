Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to perform a health check on the doctors' portal

@pipeline @release
Scenario: Doctors' Portal Health Check
    When I log in to the doctors' portal
    And the content is displayed for the DMER dashboard
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And the content is displayed for the ICBC tombstone data
    Then I log out of the portal