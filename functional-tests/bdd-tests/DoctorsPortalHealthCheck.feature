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