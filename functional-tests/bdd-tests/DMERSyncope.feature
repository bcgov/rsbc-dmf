Feature: DMERSyncope.feature
    As a Driver Medical Fitness SME
    I want to confirm the syncope business rules for a DMER

Scenario: S3DMFT-1155 - Non-Commercial Syncope Unexplained Single Not Within 7 Days > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Cause Remains Unexplained radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1156 - Non-Commercial Syncope Unexplained Single Less than 7 Days > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Cause Remains Unexplained radio button
    And I click on the Single Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 7 days    
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1157 - Non-Commercial Syncope Unexplained Recurrent Not Within 3 Months > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Cause Remains Unexplained radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on No for Syncopal Event in the past 3 months 
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment   
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1158 - Non-Commercial Syncope Unexplained Recurrent Within 3 Months > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Cause Remains Unexplained radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 3 months 
    And I enter the recurrent unexplained past year syncope details
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1159 - Non-Commercial Syncope Currently Untreated Single > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Currently Untreated radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1160 - Non-Commercial Syncope Currently Untreated Recurrent > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Currently Untreated radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1161 - Non-Commercial Syncope Diagnosed, Treated Successfully, Single, Within 7 Days > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 7 days    
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1162 - Scenario: Non-Commercial Syncope Diagnosed, Treated Successfully, Single, Not Within 7 Days > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past 7 days    
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1163 - Non-Commercial Syncope Diagnosed, Treated Successfully, Recurrent, Not Within 7 Days > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on No for Syncopal Event in the past 7 days    
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1164 - Non-Commercial Syncope Diagnosed, Treated Successfully, Recurrent, Within 7 Days > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 7 days    
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1165 - Non-Commercial Syncope Reversible, Treated Successfully, Single > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Reversible, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1166 - Non-Commercial Syncope Reversible, Treated Successfully, Recurrent > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I expand the Syncope area
    And I click on the Reversible, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment 
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1167 - Non-Commercial Syncope Situational, Avoidable Trigger, Single, Not Within 7 Days
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Situational, Avoidable Trigger radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1168 - Non-Commercial Syncope Situational, Avoidable Trigger, Single, Within 7 Days
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Situational, Avoidable Trigger radio button
    And I click on the Single Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1169 - Non-Commercial Syncope Vasovagal, Single, Typical
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Typical Vasovagal radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1170 - Non-Commercial Syncope Vasovagal, Recurrent, Atypical, Within 7 Days > Needs Review
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on Yes for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1200 - Non-Commercial Syncope Vasovagal, Recurrent, Atypical, Not Within 7 Days > Clean Pass
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
    And I do not select the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on No for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1171 - Commercial Syncope Unexplained Single No Repeat Within 7 Days > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on the Next button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment   
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1172 - Commercial Syncope Unexplained Single No Repeat Not Within 1 Year > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Cause Remains Unexplained radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1173 - Commercial Syncope Unexplained Recurrent Within 1 Year > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Cause Remains Unexplained radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1174 - Commercial Syncope Unexplained Recurrent Not Within 1 Year > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Cause Remains Unexplained radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on No for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1175 - Commercial Syncope Currently Untreated Single > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Currently Untreated radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1176 - Commercial Syncope Currently Untreated Recurrent > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Currently Untreated radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1177 - Commercial Syncope Diagnosed, Treated Successfully, Single, Within Past Month > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past month  
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1178 - Commercial Syncope Diagnosed, Treated Successfully, Single, Not Within 30 Days > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past month 
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1179 - Commercial Syncope Diagnosed, Treated Successfully, Recurrent, Not Within 30 Days > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on No for Syncopal Event in the past month
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1180 - Commercial Syncope Diagnosed, Treated Successfully, Recurrent, Within 30 Days > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Diagnosed, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past month
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1181 - Commercial Syncope Reversible, Treated Successfully, Single > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Reversible, Treated Successfully radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1182 - Commercial Syncope Reversible, Treated Successfully, Recurrent > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Reversible, Treated Successfully radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1183 - Commercial Syncope Situational, Avoidable Trigger, Single, Not Within 7 Days > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Situational, Avoidable Trigger radio button
    And I click on the Single Syncopal Event radio button
    And I click on No for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1184 - Commercial Syncope Situational, Avoidable Trigger, Single, Within 7 Days > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Situational, Avoidable Trigger radio button
    And I click on the Single Syncopal Event radio button
    And I click on Yes for Syncopal Event in the past 7 days
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1185 - Commercial Syncope Vasovagal, Single, Typical > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Typical Vasovagal radio button
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1186 - Commercial Syncope Vasovagal, Single, Atypical, Within Past Year > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on Yes for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1194 - Commercial Syncope Vasovagal, Single, Atypical, Not Within Past Year > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Single Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on No for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1195 - Commercial Syncope Vasovagal, Recurrent, Typical, Within Past Year > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Typical Vasovagal radio button
    And I click on Yes for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1196 - Commercial Syncope Vasovagal, Recurrent, Typical, Not Within Past Year > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Typical Vasovagal radio button
    And I click on No for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1197 - Commercial Syncope Vasovagal, Recurrent, Atypical, Within Past Year > Needs Review
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on Yes for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment     
    And I click on the form submit button
    Then I log out of the portal

Scenario: S3DMFT-1198 - Commercial Syncope Vasovagal, Recurrent, Atypical, Not Within Past Year > Clean Pass
    When I log in to the doctors' portal
    And I click on the DMER Forms tab
    And I click on the Case ID for 444
    And I refresh the page
    And I wait for the drivers licence field to have a value
    And I click on the Next button
    And the second page content is displayed
    And I click on the Next button
    And I enter the Uncorrected Binocular Vision as 20
    And I click on the Next button
    And I click on the Commercial DMER option
    And I click on the Syncope checkbox
    And I click on the Vasovagal Syncope radio button
    And I click on the Recurrent Syncopal Event radio button
    And I click on the Atypical Vasovagal radio button
    And I click on No for Syncopal Event in the past year
    And I click on the Next button
    And I select Yes for patient is fit to drive without additional accommodation or specialist input
    And I select No for an Enhanced Road Assessment or Road Test should be performed for this driver
    And I select No for additional specialist reports supporting this review provided for assessment    
    And I click on the form submit button
    Then I log out of the portal