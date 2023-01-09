Feature: User logs into the User Service Tests

Scenario: User attempts to login with correct DfE Admin credentials
    Given a DfE Admin user is on the Login page and is entering the correct username and password
    When the DfE Admin enters username as 'DfeAdmin@email.com' and password as 'Pass123$' and clicks login button
    Then the user clicks on the login button they should be redirect to the home page and the heading should say 'DfEAdmin' and subheading should say 'Department for Education'

Scenario: DfE Admin logins and navigates to Manage LA and VCS
    Given a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations
    When the DfE Admin clicks on the 'Manage local authorities and voluntary community organisations' link
    Then the DfE Admin is redirected to View Organisations Page

Scenario Outline: DfEAdmin on the View Organisations Uses Filters
    Given a DfE Admin is on the View Organisations page
    When the DfE Admin selects the Filter
    Then I should select "<value>" with "<text>"
    Examples:
        | value | text |
        | LA | Local Authority |
        #| VCFS | Voluntary, Charitable, Faith Sector
        | FamilyHub | Family Hub |
        #| Company | Public / Private Company eg: Child Care Centre |

