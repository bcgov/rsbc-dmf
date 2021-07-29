Feature: DMERSpecialCharacters.feature
    As a medical professional
    I want to submit a DMER with special characters

Scenario: DMER Special Characters
    When I log in to the doctors' portal
    And I click on the DMER link
    And I enter special characters into the fields
    And I submit the DMER form
    Then the DMER is successfully processed