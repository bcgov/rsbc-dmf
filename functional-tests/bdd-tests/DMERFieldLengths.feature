Feature: DMERFieldLengths.feature
    As a medical professional
    I want to submit a DMER with maximum, minimum, and required fields

Scenario: DMER Maximum Length Met
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter maximum length data into the fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Maximum Lengths Exceeded
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter data exceeding the maximum lengths into the fields
    And I submit the DMER form
    Then the DMER is not successfully processed

Scenario: DMER Minimum Length Met
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter minimum length data into the fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Minimum Lengths Not Met
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter data not equal to the minimum lengths into the fields
    And I submit the DMER form
    Then the DMER is not successfully processed

Scenario: DMER DL Length Not Met
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter DL number not equal to seven digits
    And I submit the DMER form
    Then the DMER is not successfully processed

Scenario: DMER Required Fields Completed
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter data that only includes the required fields
    And I submit the DMER form
    Then the DMER is successfully processed

Scenario: DMER Required Fields Not Filled
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I enter data that does not include the required fields
    And I submit the DMER form
    Then the DMER is not successfully processed