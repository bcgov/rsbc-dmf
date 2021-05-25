Feature: DMERFieldLengths.feature
    As a medical professional
    I want to submit a DMER with maximum, minimum, and required fields

Scenario: DMER Maximum Length Met
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter maximum length data into the fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Maximum Lengths Exceeded
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter data exceeding the maximum lengths into the fields
    And I submit the DMER form
    Then the DMER is not successfully processed

Scenario: DMER Minimum Length Met
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter minimum length data into the fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Minimum Lengths Not Met
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter data not equal to the minimum lengths into the fields
    And I submit the DMER form
    Then the DMER is not successfully processed

Scenario: DMER Required Fields Completed
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter data that only includes the required fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Required Fields Not Filled
    Given I am logged in to the PHSA Portal
    And I click on the DMER link
    And I enter data that does not include the required fields
    And I submit the DMER form
    Then the DMER is not successfully processed