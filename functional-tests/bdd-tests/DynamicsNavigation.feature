Feature: DynamicsNavigation
    As an DMFT team member
    I want to practice navigation in the Dynamics portal

@manualonly @temporary
Scenario: Dynamics Participants Navigation
    When I log in to the Dynamics portal
    And I wait for the Dynamics homepage content to be displayed
    And I click on a case under Active Cases
    And the case details are displayed on the left hand side
    And I click on the Related tab
    And I click on the Participants option
    And the Participant Associated View is displayed
    And the Full Name, Role, and Created On date are displayed
    Then I log out of the portal

@manualonly @temporary
Scenario: Dynamics DMERs Navigation
    When I log in to the Dynamics portal
    And I wait for the Dynamics homepage content to be displayed
    And I click on a case under Active Cases
    And the case details are displayed on the left hand side
    And I click on the Related tab
    And I click on the DMERs option
    And I click on the DMER Id
    And the DMER Id and the Owner are displayed
    Then I log out of the portal

@manualonly @temporary
Scenario: Dynamics Decisions Navigation
    When I log in to the Dynamics portal
    And I wait for the Dynamics homepage content to be displayed
    And I click on a case under Active Cases
    And the case details are displayed on the left hand side
    And I click on the Related tab
    And I click on the Decisions option
    And I click on the Decision Name
    And the Decision Name and the Owner are displayed
    Then I log out of the portal