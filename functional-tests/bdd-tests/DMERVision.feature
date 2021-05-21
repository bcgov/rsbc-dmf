Feature: DMERVision.feature
    As a Driver Medical Fitness SME
    I want to confirm the vision business rules for a DMER

Scenario: Non-Commercial DMER Vision Not 20/50
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'No' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Vision Not 20/30
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'No' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Visual Field Defects
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I select 'Yes' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Visual Field Defects
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I select 'Yes' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Visual Acuity
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'Yes' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Visual Acuity
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'Yes' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Monocular Vision
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'Yes' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Monocular Vision
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'Yes' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Does Not Meet Physician's Guide Criteria
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'No' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Does Not Meet Physician's Guide Criteria
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'Yes' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'No' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Uncorrected Binocular Vision Contradicts 20/50 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/55
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Uncorrected Left Eye Vision Contradicts 20/50 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Left Eye Vision as 20/55
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Non-Commercial DMER Uncorrected Right Eye Vision Contradicts 20/50 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Right Eye Vision as 20/55
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Uncorrected Binocular Vision Contradicts 20/30 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/35
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Uncorrected Left Eye Vision Contradicts 20/30 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Left Eye Vision as 20/35
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed

Scenario: Commercial DMER Uncorrected Right Eye Vision Contradicts 20/30 Statement
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Right Eye Vision as 20/35
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER must be reviewed