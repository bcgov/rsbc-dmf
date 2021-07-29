Feature: DMERReturnToPreviousStep.feature
    As a medical professional
    I want to confirm that I can return to previous steps of a DMER

Scenario: DMER Previous Step
    When I log in to the doctors' portal
    And I click on the DMER link
    And I complete the DMER fields
    And I return to a previous step
    Then the page has retained the previously entered data