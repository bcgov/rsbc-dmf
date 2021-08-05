Feature: DynamicsPortalHealthCheck
    As a medical professional
    I want to perform a health check on the Dynamics portal

@pipeline
Scenario: Dynamics Portal Authentication
    When I log in to the Dynamics portal
    And I wait for the Dynamics homepage content to be displayed
    Then I log out of the portal