Feature: DMERFieldValidation.feature
    As a Driver Medical Fitness SME
    I want to confirm the validation business rules for a DMER

Scenario: Non-Commercial DMER Patient Last Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Last Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient First Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient First Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Date of Birth Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's date of birth is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Date of Birth Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's date of birth is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Gender Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's gender is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Gender Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's gender is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient City Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's city is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient City Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's city is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Street Address 1 Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's street address line 1 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Street Address 1 Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's street address line 1 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Street Address 2 Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    # Requirement of Street Address 2 to be confirmed
    And the patient's street address line 2 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Street Address 2 Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    # Requirement of Street Address 2 to be confirmed
    And the patient's street address line 2 is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Patient Postal Code Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the patient's postal code is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Patient Postal Code Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the patient's postal code is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Empty DMER Doesn't Save
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not completed
    And I submit the DMER form
    Then the DMER does not save

Scenario: Non-Commercial DMER Provider First Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider First Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's first name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider Last Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider Last Name Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's last name is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider ID Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's ID is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider ID Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's ID is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider ID Type Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's ID type is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider ID Type Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's ID type is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Provider Role Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And the provider's role is missing
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Provider Role Missing
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And the provider's role is missing
    And I submit the DMER form
    Then the DMER must be reviewed