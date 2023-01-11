Feature: User logs into the User Service in order to send a user an invitation to create an account


Scenario: DfE Admin logins and uses Create user accounts wizard by pressing on the link in the homepage
    Given DfE Admin logins and uses Create user accounts wizard
    When the user press the 'Create user accounts' link
    Then the DfE Admin is redirected from the Homepage to the Which account do you want to create? page

Scenario: DfE Admin logins and navigates to Which account do you want to create? Page and forgets to select one of the radio buttons
    Given DfE Admin logins and navigates to Which account do you want to create? Page and and forgets to select one of the radio buttons
    When the DfE Admin clicks the Continue button
    Then the error message for Which account do you want to create? Page appear with 'You must select an account type' and 'Select the type of account you want to create'

Scenario: DfE Admin logins and uses Create user accounts wizard to create a Department for Education administrator user
    Given DfE Admin logins and uses Create user accounts wizard and navigates to the Which account do you want to create? page
    When the DfE Admin clicks the on the Department for Education administrator radio button and then the continue button
    Then the DfE Admin is redirected from the Which account do you want to create? to the 'What is the user's full name?' Page

Scenario Outline: DfE Admin logins and uses Create user accounts wizard and selects one of the radio buttons
    Given DfE Admin logins and navigates to Which account do you want to create? Page and selects one of the radio buttons
    When the DfE Admin clicks the on the radio button with "<text>" and has "<value>"
    Then the DfE Admin is redirected to the 'What is the user's full name?' Page
    When the DfE Admin clicks on the continue button for the Which local authority is the account for? page
    Then the DfE Admin is redirected from the Which local authority is the account for? page to the 'What is the user's full name?' Page
    Examples:
        | value | text |
        | LAAdmin | Local authority administrator |
        | VCSAdmin | Voluntary community organisation administrator |
        | Professional | Professional |

Scenario: DfE Admin logins and uses Create user accounts wizard, arrives at What is the user's full name? page, and presses continue without adding a name
    Given DfE Admin logins and uses Create user accounts wizard, arrives at What is the user's full name? page
    When the DfE Admin clicks the Continue button on the at What is the user's full name? page without adding a name
    Then On the What is the user's full name? page the error message 'Enter a name' appears

Scenario: DfE Admin logins and uses Create user accounts wizard, arrives at What is the user's full name? page, and enters the users name then presses continue
    Given DfE Admin logins and uses Create user accounts wizard with a user type of LAAdmin arrives at What is the user's full name? page
    When the DfE Admin adds the user name 'Joe Bloggs' and then clicks the Continue button
    Then the DfE Admin is redirected from the 'What is the user's full name?' page to the 'What is the user's email address?' Page

Scenario: DfE Admin logins and navigates to 'What is the user's email address?' Page and forgets to enter an email address jut presses continue button
    Given DfE Admin logins and navigates to 'What is the user's email address?' Page and forgets to enter an email address
    When the DfE Admin while on the 'What is the user's email address?' Page just clicks the Continue button
    Then the error message for 'What is the user's email address?' Page appear with 'Enter an email address' and 'Enter an email address'

Scenario: DfE Admin logins and navigates to 'What is the user's email address?' Page and enters the users email address and presses continue button
    Given DfE Admin logins and navigates to 'What is the user's email address?' Page in the Create user accounts wizard
    When the DfE Admin enters the users email address 'joe.blogs@email.com' and then clicks the Continue button
    Then the DfE Admin is redirected from the 'What is the user's email address?' Page to the Check account details page

Scenario: DfE Admin logins and uses Create user accounts wizard reaching the Check account details page then presses back buttons back to the home page
    Given DfE Admin logins and uses Create user accounts wizard reaching the Check account details page
    When the DfE Admin presses the Check account details back button
    Then the DfE Admin is redirected from the the Check account details page to the 'What is the user's email address?' Page
    When the DfE Admin presses the 'What is the user's email address?' back button
    Then the DfE Admin is redirected from the the 'What is the user's email address?' Page to the 'What is the user's full name?' page
    When the DfE Admin presses the 'What is the user's full name?' back button
    Then the DfE Admin is redirected from the 'What is the user's full name?' page to the Which local authority is the account for? page
    When the DfE Admin presses the Which local authority is the account for? back button
    Then the DfE Admin is redirected from the 'Which local authority is the account for?' page to the Which account do you want to create? page
    When the DfE Admin presses the Which account do you want to create? back button
    Then the DfE Admin is redirected from the 'Which account do you want to create?' page to the home page


        

