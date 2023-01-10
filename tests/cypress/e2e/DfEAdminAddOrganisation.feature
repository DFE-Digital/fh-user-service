Feature: DfE Admin logs into the User Service and Adds New Organisation



Scenario: DfE Admin logins and navigates to Adds New Organisation Page
    Given a DfE Admin logs in and goes to the Add a local authority or voluntary community organisation wizard
    When the DfE Admin clicks on the 'Add a local authority or voluntary community organisation' link
    Then the DfE Admin is redirected to the Type Of Organisation Page

Scenario: DfE Admin logins and navigates to Add New Organisation Page and then goes back
    Given DfE Admin logins and navigates to Add New Organisation Page  and then wants to go back
    When the DfE Admin clicks on the back link
    Then the DfE Admin is redirected back to the Home page Page

Scenario: DfE Admin logins and navigates to Add New Organisation Page and forgets to select one of the radio buttons
    Given DfE Admin logins and navigates to Add New Organisation Page and and forgets to select one of the radio buttons
    When the DfE Admin clicks the Continue button
    Then the error message 'You must select an organisation type' and 'Select the type of organisation you want to create' appear

Scenario Outline: DfE Admin logins and navigates to Add New Organisation Page and select one of the radio buttons
    Given DfE Admin logins and navigates to Add New Organisation Page and select one of the radio buttons
    When the DfE Admin clicks the on the radio button with "<text>" and has "<value>"
    Then the DfE Admin is redirected to the 'Which local authority is the organisation in?' Page
    Examples:
        | value | text |
        | LA | Local Authority |
        | VCFS | Voluntary, Charitable, Faith Sector |
        | Company | Public / Private Company eg: Child Care Centre |

Scenario: DfE Admin logins and navigates to Which local authority is the organisation in? Page makes selection and continues
    Given DfE Admin logins and navigates to Which local authority is the organisation in? Page makes selection and continues
    When the DfE Admin clicks the Continue button on the Which local authority is the organisation in? Page
    Then the DfE Admin is redirected to the 'What is the local authority's name?' Page
        

