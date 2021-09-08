Feature: DMERCardio.feature
    As a Driver Medical Fitness SME
    I want to confirm the cardio business rules for a DMER

Scenario: Cardio All Fields (Non-Commercial)
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 333
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I enter the cardio steps
    And I click on the Next button
    And I enter the medical opinion and confirmations
    And I click on the form submit button
    Then I log out of the portal