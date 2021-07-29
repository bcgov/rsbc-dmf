Feature: DMERFieldValidation.feature
    As a Driver Medical Fitness SME
    I want to confirm the validation business rules for a DMER

Scenario: Non-Commercial DMER Patient Last Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Last Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient First Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient First Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Date of Birth Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's date of birth is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Date of Birth Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's date of birth is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Gender Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's gender is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Gender Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's gender is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient City Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's city is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient City Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's city is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Street Address 1 Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's street address line 1 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Street Address 1 Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's street address line 1 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Street Address 2 Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    # Requirement of Street Address 2 to be confirmed
    And the patient's street address line 2 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Street Address 2 Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    # Requirement of Street Address 2 to be confirmed
    And the patient's street address line 2 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Postal Code Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's postal code is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Postal Code Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's postal code is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Empty DMER Doesn't Save
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not completed
    And I submit the DMER form
    Then the DMER does not save

Scenario: Non-Commercial DMER Provider First Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider First Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider Last Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider Last Name Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider ID Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's ID is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider ID Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's ID is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider ID Type Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's ID type is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider ID Type Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's ID type is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider Role Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's role is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider Role Missing
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's role is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: DMER Known Medical Conditions Populated
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And known medical conditions have been identified
    Then the corresponding medical sections are displayed

Scenario: Commercial DMER Copyedit Complete
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    Then the copyedit for all content is complete

Scenario: Non-Commercial DMER Copyedit Complete
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    Then the copyedit for all content is complete

Scenario: DMER Medical Terminology Correct
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    Then the medical copyedit for all content is complete

Scenario: Commercial DMER Tool Tips Provided
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    Then the correct tool tips are provided

Scenario: Non-Commercial DMER Tool Tips Provided
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    Then the correct tool tips are provided

Scenario: Commercial DMER Download As PDF
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the DMER is completed
    Then the DMER can be downloaded as a PDF

Scenario: Non-Commercial DMER Download As PDF
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the DMER is completed
    Then the DMER can be downloaded as a PDF

Scenario: DMER Search by Valid Case ID
    When I log in to the doctors' portal
    And I enter a valid DMER Case ID
    Then the correct DMER is displayed

Scenario: DMER Search by Valid Driver License Number
    When I log in to the doctors' portal
    And I enter a valid Driver License Number
    Then the correct DMER is displayed

Scenario: DMER Search by Invalid Case ID
    When I log in to the doctors' portal
    And I enter an invalid DMER Case ID
    Then an error message is displayed on the search page

Scenario: DMER Search by Invalid Driver License Number
    When I log in to the doctors' portal
    And I enter an invalid Driver License Number
    Then an error message is displayed on the search page