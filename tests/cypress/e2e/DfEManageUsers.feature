Feature: DfE Admin logs into the User Service and Manages Users

Scenario: DfE Admin logins and navigates to Manage user accounts Page
    Given a DfE Admin logs in and goes to the Manage user accounts Page
    When the DfE Admin clicks on the 'Manage user accounts' link
    Then the DfE Admin is redirected to Manage user accounts Page

Scenario: DfE Admin logins and navigates to Manage user accounts Page and then go back
    Given DfE Admin logins and navigates to Manage user accounts Page  and then wants to go back
    When the DfE Admin clicks on the Manage user accounts back link
    Then the DfE Admin is redirected back to the Home page Page


        

