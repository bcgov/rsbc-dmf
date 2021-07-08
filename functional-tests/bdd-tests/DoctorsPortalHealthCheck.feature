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

Scenario: Vision Assessment
    When I click on the doctors' portal
    And the portal is displayed
    And I enter the login credentials
    And I click on the Submit button
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I select the RoadSafetyBC environment
    And I select the Testing Resources Quality Assurance form
    And I click on the Visual Assessment tab
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Next button
    And I click on the Next button
    And I click on the Next button
    Then this message is displayed: PASS! No Clean Pass responses failed.