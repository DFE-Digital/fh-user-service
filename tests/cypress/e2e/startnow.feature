Feature: User Service Tests Goes To Start Now Page then Logs in

Scenario: home page heading is 'Manage family support services and accounts'
    Given a user has arrived on user services home page
    Then the heading should say 'Manage family support services and accounts'

 Scenario: user service heading is 'Manage family support services and accounts'
    Given a user has arrived at the user services home page
    Then the user clicks on start now button should redirect to login page

Scenario: Login page heading is 'Sign in to your account'
    Given a user has arrived at the login page
    Then the login page heading should say 'Sign in to your account' and the subheading should say 'Use a local account to log in.'

Scenario: User attempts to login with a username that is not an email address
    Given a user is on the Login page
    When the user enters username as 'username' and clicks login button
    Then the error message 'The Email field is not a valid e-mail address.' and 'The Password field is required.' is displayed

Scenario: User attempts to login with incorrect username or password
    Given a user is on the Login page and is entering either the wrong username or password
    When the user enters username as 'username@email.com' and password as 'password' and clicks login button
    Then the error message 'Invalid login attempt.' is displayed

Scenario: User attempts to login with correct DfE Admin credentials
    Given a DfE Admin user is on the Login page and is entering the correct username and password
    When the DfE Admin enters username as 'DfeAdmin@email.com' and password as 'Pass123$' and clicks login button
    Then the user clicks on the login button they should be redirect to the home page



 

