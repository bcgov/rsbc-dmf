Feature: DMERValidationRules.feature
    As a medical professional
    I want to confirm the business rules for a DMER

Scenario: DMER Validation Rules
    When I log in to the doctors' portal
    And I click on the DMER link
    And I do not complete the DMER fields
    And I submit the DMER form
    Then the DMER validation rules are displayed