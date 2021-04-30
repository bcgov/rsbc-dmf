Feature: DMERSaveForLater.feature
    As a medical professional
    I want to confirm that I can save a DMER for later

Scenario: DMER Save For Later
    Given I am logged in to the RSBC Portal
    And I click on the DMER link for the patient named Sam McDonald
    And I complete the DMER fields
    And I click on the Save For Later button
    And I click on the DMER link for the patient named Sam McDonald
    Then the previously entered data is displayed