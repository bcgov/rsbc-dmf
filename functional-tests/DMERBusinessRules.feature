Feature: PortalCompleteDMER.feature
    As a medical professional logged into the RSBC Portal
    I want to submit a DMER for my patient

Scenario: Portal Complete DMER 
    Given I am logged in to the RSBC Portal
    And I click on the DMER link for the patient named Sam McDonald
    And I complete the DMER form
    And I submit the DMER form
    Then I see the DMER is processed