Feature: PortalCompleteDMER.feature
    As a medical professional logged into the RSBC Portal
    I want to submit a DMER for my patient

Scenario: Portal Complete DMER 
    When I log in to the doctors' portal
    And I click on the DMER link for the patient
    And I complete the DMER form
    And I submit the DMER form
    Then I see the DMER is processed