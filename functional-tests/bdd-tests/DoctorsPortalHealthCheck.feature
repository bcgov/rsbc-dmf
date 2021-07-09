Feature: DoctorsPortalHealthCheck
    As a medical professional
    I want to test the doctors' portal

@pipeline
Scenario: Doctors' Portal Health Check
    When I click on the doctors' portal
    And the content is displayed for the doctors portal
    And I enter the login credentials
    And I click on the Submit button
    And the content is displayed for the DMER dashboard
    Then I log out of the portal

@browseronly
Scenario: Doctors' Portal Health Check with Cert
    When I click on the doctors' portal
    And I accept the cert request
    And the content is displayed for the doctors portal
    And I enter the login credentials
    And the content is displayed for the DMER dashboard
    Then I log out of the portal

Scenario: Vision Assessment
    When I click on the doctors' portal
    And I accept the cert request
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
    And the content is displayed for the DMER clean pass
    Then I log out of the portal