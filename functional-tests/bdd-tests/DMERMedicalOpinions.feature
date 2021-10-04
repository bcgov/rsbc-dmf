Feature: DMERMedicalOpinions.feature
    As a medical professional
    I want to submit a DMER with varying medical opinions

Scenario: Medical Opinions > Clean Pass
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
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal

Scenario: Medical Opinions > Not Fit To Drive > Needs Review
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
    And I click on the Next button
    And I select No for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal

Scenario: Medical Opinions > Road Test Required > Needs Review
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
    And I click on the Next button
    And I select No for patient is fit to drive without additional accommodation or specialist input
    And I select Yes for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal

Scenario: Medical Opinions > Specialist Reports > Needs Review
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
    And I click on the Next button
    And I select No for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select Yes for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal