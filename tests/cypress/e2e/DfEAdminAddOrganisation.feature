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

Scenario: DfE Admin logins and uses Add New Organisation wizard,  arrives at What is the local authority's name?, and presses continue without adding a name
    Given DfE Admin logins and uses Add New Organisation wizard, arrives at What is the local authority's name? page
    When the DfE Admin clicks the Continue button on the at What is the local authority's name? page without adding a name
    Then On the What is the local authority's name? page the error message 'Enter a name' and 'The Name field is required.' appear

Scenario: DfE Admin logins and uses Add New Organisation wizard,  arrives at What is the local authority's name?, types a name and presses continue
    Given DfE Admin logins and uses Add New Organisation wizard, progresses to the What is the local authority's name? page
    When at What is the local authority's name? page, the DfE Admin types a name 'Test LA' and clicks the Continue button
    Then the DfE Admin is redirected to the Check details Page having a heading of 'Check details'
        
Scenario: DfE Admin logins and uses Add New Organisation wizard and then presses the back button on each page
    Given DfE Admin logins and uses Add New Organisation wizard through to the check details page
    When the user press the back button on the Check Details page
    Then the DfE Admin is redirected to the What is the local authority's name? page
    When the user press the next back button on the What is the local authority's name? page
    Then the DfE Admin is redirected from the What is the local authority's name? page to the Which local authority is the organisation in? page
    When the user press the next back button on the Which local authority is the organisation in? page
    Then the DfE Admin is redirected from the Which local authority is the organisation in? page to the Type of Organisation page

Scenario: DfE Admin logins and uses Add New Organisation wizard and then presses the change link for What you added
    Given DfE Admin logins and uses Add New Organisation wizard and wants to change the 'What you added' entry
    When the user press the change link for What you added
    Then the DfE Admin is redirected from the Check Details page to the Type of Organisation page

Scenario: DfE Admin logins and uses Add New Organisation wizard and then presses the change link for Local Authority
    Given DfE Admin logins and uses Add New Organisation wizard and wants to change the 'Local Authority' entry
    When the user press the change link for Local Authority
    Then the DfE Admin is redirected from the Check Details page to the Which local authority is the organisation in? page

Scenario: DfE Admin logins and uses Add New Organisation wizard and then presses the change link for Name
    Given DfE Admin logins and uses Add New Organisation wizard and wants to change the 'Name' entry
    When the user press the change link for Name of the organisation
    Then the DfE Admin is redirected from the Check Details page to the What is the local authority's name? page

Scenario: DfE Admin logins and uses Add New Organisation wizard and then presses the Confirm details button
    Given DfE Admin logins and uses Add New Organisation wizard and enters all the data
    When the user press the Confirm details button
    Then the DfE Admin is redirected from the Check Details page to the Confirmation page