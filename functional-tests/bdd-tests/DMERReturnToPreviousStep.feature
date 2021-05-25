Feature: DMERReturnToPreviousStep.feature
    As a medical professional
    I want to confirm that I can return to previous steps of a DMER

Scenario: DMER Previous Step
    Given I am logged in to the RSBC Portal
    And I click on the DMER link
    And I complete the DMER fields
    And I return to a previous step
    Then the page has retained the previously entered data