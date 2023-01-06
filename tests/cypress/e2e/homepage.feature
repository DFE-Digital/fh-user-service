Feature: Home Page Page Tests
  Scenario: home page url is '/'
    Given a user has arrived on the home page 
    Then the page URL ends with '/'

   Scenario: home page heading is 'Manage family support services and accounts'
    Given a user has arrived on the home page
    Then the heading should say 'Manage family support services and accounts'

   Scenario: home page has a start now button 
    Given a user has arrived on the home page
    Then they see the start now button


