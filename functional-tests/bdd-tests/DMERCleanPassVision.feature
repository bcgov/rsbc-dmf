Feature: DMERCleanPass.feature
    As a Driver Medical Fitness SME
    I want to confirm the vision only clean pass business rules for a DMER

Scenario: Clean Pass Vision Assessment
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 111
    And I refresh the page
    And I click on the Next button
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And the content is displayed for the DMER clean pass
    Then I log out of the portal

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/20
#    And I enter the Uncorrected Left Eye Vision as 20/20
#    And I enter the Uncorrected Right Eye Vision as 20/20
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/20
#    And I enter the Uncorrected Left Eye Vision as 20/20
#    And I enter the Uncorrected Right Eye Vision as 20/20
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/50)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/50
#    And I enter the Uncorrected Left Eye Vision as 20/50
#    And I enter the Uncorrected Right Eye Vision as 20/50
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/45)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/45
#    And I enter the Uncorrected Left Eye Vision as 20/45
#    And I enter the Uncorrected Right Eye Vision as 20/45
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/40)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/40
#    And I enter the Uncorrected Left Eye Vision as 20/40
#    And I enter the Uncorrected Right Eye Vision as 20/40
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/35)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/35
#    And I enter the Uncorrected Left Eye Vision as 20/35
#    And I enter the Uncorrected Right Eye Vision as 20/35
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/30)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/30
#    And I enter the Uncorrected Left Eye Vision as 20/30
#    And I enter the Uncorrected Right Eye Vision as 20/30
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/25)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/25
#    And I enter the Uncorrected Left Eye Vision as 20/25
#    And I enter the Uncorrected Right Eye Vision as 20/25
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/20)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/20
#    And I enter the Uncorrected Left Eye Vision as 20/20
#    And I enter the Uncorrected Right Eye Vision as 20/20
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/15)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/15
#    And I enter the Uncorrected Left Eye Vision as 20/15
#    And I enter the Uncorrected Right Eye Vision as 20/15
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Non-Commercial DMER Vision Only Clean Pass (Happy Path - 20/10)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is not to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/50 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/10
#    And I enter the Uncorrected Left Eye Vision as 20/10
#    And I enter the Uncorrected Right Eye Vision as 20/10
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/30)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/30
#    And I enter the Uncorrected Left Eye Vision as 20/30
#    And I enter the Uncorrected Right Eye Vision as 20/30
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/25)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/25
#    And I enter the Uncorrected Left Eye Vision as 20/25
#    And I enter the Uncorrected Right Eye Vision as 20/25
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/20)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/20
#    And I enter the Uncorrected Left Eye Vision as 20/20
#    And I enter the Uncorrected Right Eye Vision as 20/20
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/15)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/15
#    And I enter the Uncorrected Left Eye Vision as 20/15
#    And I enter the Uncorrected Right Eye Vision as 20/15
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass

#Scenario: Commercial DMER Vision Only Clean Pass (Happy Path - 20/10)
#    When I log in to the doctors' portal
#    And I click on the DMER Forms tab
#    And I click on the Case ID for 111
#    And I refresh the page
#    And I click on the Preliminary Visual Assessment tab
#    And the DMER is to be processed for commercial purposes
#    And I select 'Yes' for 'Binocular vision, Corrected or Uncorrected is 20/30 or better?'
#    And I enter the Uncorrected Binocular Vision as 20/10
#    And I enter the Uncorrected Left Eye Vision as 20/10
#    And I enter the Uncorrected Right Eye Vision as 20/10
#    And I do not select the corrected vision checkbox
#    And I select 'No' for 'Has the patient experienced any visual field defects?'
#    And I select 'No' for 'Has the patient experienced loss of visual acuity?'
#    And I select 'No' for 'Has the patient been diagnosed with monocular vision?'
#    And I select 'Yes' for 'Both Visual Acuity and Visual Field meet Physician’s Guide criteria for license class'
#    And I submit the DMER form
#    Then the DMER has a clean pass