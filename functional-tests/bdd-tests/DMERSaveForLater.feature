Feature: DMERSaveForLater.feature
    As a medical professional
    I want to confirm that I can save a DMER for later

Scenario: DMER Save For Later
    When I log in to the doctors' portal
    And I click on the DMER link
    And I complete the DMER fields
    And I click on the Save For Later button
    And I click on the DMER link
    Then the previously entered data is displayed