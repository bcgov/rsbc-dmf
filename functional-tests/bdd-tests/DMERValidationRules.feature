Feature: DMERValidationRules.feature
    As a medical professional
    I want to confirm the business rules for a DMER

Scenario: DMER Validation Rules
    Given I am logged in to the RSBC Portal
    And I click on the DMER link for the patient named Sam McDonald
    And I do not complete the DMER fields
    And I submit the DMER form
    Then the DMER validation rules are displayed