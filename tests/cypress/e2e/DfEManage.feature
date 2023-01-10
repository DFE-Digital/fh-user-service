Feature: User logs into the User Service Tests

Scenario: User attempts to login with correct DfE Admin credentials
    Given a DfE Admin user is on the Login page and is entering the correct username and password
    When the DfE Admin enters username as 'DfeAdmin@email.com' and password as 'Pass123$' and clicks login button
    Then the user clicks on the login button they should be redirect to the home page and the heading should say 'DfEAdmin' and subheading should say 'Department for Education'

Scenario: DfE Admin logins and navigates to Manage LA and VCS
    Given a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations
    When the DfE Admin clicks on the 'Manage local authorities and voluntary community organisations' link
    Then the DfE Admin is redirected to View Organisations Page

Scenario: DfE Admin logins and navigates to Manage LA and VCS and then back
    Given a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations and then wants to go back
    When the DfE Admin clicks on the ViewOrganisations back link
    Then the DfE Admin is redirected back to the View Organisations Page

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

Scenario: DfE Admin logins and navigates to Manage LA and VCS
    Given a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations page
    When the DfE Admin clicks all the filters follwed by apply filter button
    Then presses clear filters link and the apply filter button, all results are shown

Scenario: DfE Admin logins and navigates to Manage LA and VCS and Views and Item in the list
    Given a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page to view details
    When the DfE Admin clicks on the Tower Hamlets View Link
    Then the DfE Admin is redirect to the details page with the heading as 'Home-Start'

Scenario: DfE Admin logins and navigates to Manage LA and VCS and Views and Item then goes back to previous page
    Given a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page to view item details
    When the DfE Admin clicks on Back to manage local authorities and voluntary community organisations
    Then the DfE Admin is redirect back to the Manage local authorities and voluntary community organisations page

Scenario: DfE Admin logins and navigates to Manage LA and VCS and Views and Item then alters organisation name
    Given a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page then alters organisation name
    When the DfEAdmin should modify the name to "<value>" and press save details
    Then Local Authority name should be changed to "<value>"
    Examples:
        | value |
        | Tower Hamlets CouncilX |
        | Tower Hamlets Council |
        
Scenario: DfE Admin logins and navigates to Manage LA and VCS page then details page and then back
    Given a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations and then to the deatils page then wants to go back
    When the DfE Admin clicks on the View Organisations Details page back link
    Then the DfE Admin is redirected back to the Manage local authorities and voluntary community organisations Page
