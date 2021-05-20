Feature: DMERCleanPass.feature
    As a Driver Medical Fitness SME
    I want to confirm the vision only clean pass business rules for a DMER

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/20
    And I enter the Uncorrected Left Eye Vision as 20/20
    And I enter the Uncorrected Right Eye Vision as 20/20
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/20
    And I enter the Uncorrected Left Eye Vision as 20/20
    And I enter the Uncorrected Right Eye Vision as 20/20
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass