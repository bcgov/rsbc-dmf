Feature: EMRCompleteDMER.feature
    As a medical professional logged into an EMR solution
    I want to submit a DMER for my patient

Scenario: EMR Complete DMER 
    Given I am logged in to the EMR solution
    And I click on the DMER link for the patient named Sam McDonald
    And I complete the DMER form
    And I submit the DMER form
    Then I see the DMER is processed