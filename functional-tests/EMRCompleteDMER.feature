Feature: DMERBusinessRules.feature
    As a RSBC Driver Medical Fitness SME
    I want to validate the DMER business rules

Scenario: DMER Business Rules
    Given I am logged in to the PHSA Portal
    And I click on the DMER link for the patient named Sam McDonald
    And I do not complete any fields
    And I submit the DMER form
    Then the DMER validation is displayed