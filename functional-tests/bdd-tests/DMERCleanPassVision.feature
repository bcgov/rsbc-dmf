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
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
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
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/50)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/50
    And I enter the Uncorrected Left Eye Vision as 20/50
    And I enter the Uncorrected Right Eye Vision as 20/50
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/45)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/45
    And I enter the Uncorrected Left Eye Vision as 20/45
    And I enter the Uncorrected Right Eye Vision as 20/45
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/40)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/40
    And I enter the Uncorrected Left Eye Vision as 20/40
    And I enter the Uncorrected Right Eye Vision as 20/40
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/35)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/35
    And I enter the Uncorrected Left Eye Vision as 20/35
    And I enter the Uncorrected Right Eye Vision as 20/35
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/30)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/30
    And I enter the Uncorrected Left Eye Vision as 20/30
    And I enter the Uncorrected Right Eye Vision as 20/30
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/25)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/25
    And I enter the Uncorrected Left Eye Vision as 20/25
    And I enter the Uncorrected Right Eye Vision as 20/25
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/20)
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
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/15)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/15
    And I enter the Uncorrected Left Eye Vision as 20/15
    And I enter the Uncorrected Right Eye Vision as 20/15
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/10)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is not to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
    And I enter the Uncorrected Binocular Vision as 20/10
    And I enter the Uncorrected Left Eye Vision as 20/10
    And I enter the Uncorrected Right Eye Vision as 20/10
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/30)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/30
    And I enter the Uncorrected Left Eye Vision as 20/30
    And I enter the Uncorrected Right Eye Vision as 20/30
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/25)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/25
    And I enter the Uncorrected Left Eye Vision as 20/25
    And I enter the Uncorrected Right Eye Vision as 20/25
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/20)
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
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/15)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/15
    And I enter the Uncorrected Left Eye Vision as 20/15
    And I enter the Uncorrected Right Eye Vision as 20/15
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass

Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/10)
    Given I am logged in to the Doctors' Portal
    When I click on the DMER link for the patient
    And the DMER is to be processed for commercial purposes
    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
    And I enter the Uncorrected Binocular Vision as 20/10
    And I enter the Uncorrected Left Eye Vision as 20/10
    And I enter the Uncorrected Right Eye Vision as 20/10
    And I do not select the corrected vision checkbox
    And I select 'No' for 'Has the patient experienced any visual field defects?'
    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician�s Guide criteria for license class'
    And I submit the DMER form
    Then the DMER has a clean pass