Feature: DfE Admin logs into the User Service and Manages Users

Scenario: DfE Admin logins and navigates to Manage user accounts Page
    Given a DfE Admin logs in and goes to the Manage user accounts Page
    When the DfE Admin clicks on the 'Manage user accounts' link
    Then the DfE Admin is redirected to Manage user accounts Page

Scenario: DfE Admin logins and navigates to Manage user accounts Page and then go back
    Given DfE Admin logins and navigates to Manage user accounts Page  and then wants to go back
    When the DfE Admin clicks on the Manage user accounts back link
    Then the DfE Admin is redirected back to the Home page Page

Scenario Outline: DfEAdmin on the Manage user accounts Page and selects each filter in turn
    Given DfEAdmin on the Manage user accounts Page and selects each filter in turn
    When the DfE Admin selects the Filter
    Then I should select "<value>" with "<text>"
    Examples:
        | value | text |
        | DfEAdmin | Department for Education administrator |
        | LAAdmin | Local authority administrator |
        | VCSAdmin | Voluntary community organisation administrator |
        | Professional | Professional |

Scenario: DfE Admin logins and navigates to Manage user accounts Page and then clears the filters
    Given DfE Admin logins and navigates to Manage user accounts Page and sets all the checkbox filters
    When the DfE Admin clicks all the filters follwed by apply filter button
    Then presses the clear filters link, all the results are then shown without filters

Scenario: DfE Admin logins and navigates to Manage user accounts page and Views and Item in the list
    Given a DfE Admin logs in and navigates to Manage Manage user accounts page to view a users details
    When the DfE Admin clicks on the 'BtlLAAdmin@email.com' View Link
    Then the DfE Admin is redirect to the details page with the heading as 'BtlLAAdmin@email.com'

Scenario: DfE Admin logins and navigates to Manage user accounts page and then view item and then go to previous page
    Given a DfE Admin logs in and goes to Manage user accounts page and then to the details page then wants to go back to the previous page
    When the DfE Admin in the view user page clicks on the 'Back to manage user accounts' link
    Then the DfE Admin is redirected back to the Manage user accounts page Page

Scenario: DfE Admin logins and navigates to Manage user accounts page and then view item and then presses delete user link
    Given a DfE Admin logs in and goes to Manage user accounts page and then to the details page then presses delete user link
    When the DfE Admin in the view user page clicks on the delete user link
    Then the DfE Admin is redirected to delete user page


        

