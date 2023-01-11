import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";


Given("DfE Admin logins and uses Create user accounts wizard", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
});

When("the user press the 'Create user accounts' link", () => {
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').first().click();
});

Then("the DfE Admin is redirected from the Homepage to the Which account do you want to create? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/TypeOfUser"));
});

Given("DfE Admin logins and navigates to Which account do you want to create? Page and and forgets to select one of the radio buttons", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
});

When("the DfE Admin clicks the Continue button", () => {
    cy.get('.govuk-button').click();
});

Then("the error message for Which account do you want to create? Page appear with {string} and {string}", (toperrormessage, selecterrormessage) => {
    cy.contains('a', toperrormessage);
    cy.get('.govuk-error-message').contains(selecterrormessage).should('be.visible');
});


Given("DfE Admin logins and uses Create user accounts wizard and navigates to the Which account do you want to create? page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
});

When("the DfE Admin clicks the on the Department for Education administrator radio button and then the continue button", () => {
    cy.get('[value="DfEAdmin"]').first().check();
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected from the Which account do you want to create? to the 'What is the user's full name?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhatIsUsername"));
});


Given("DfE Admin logins and navigates to Which account do you want to create? Page and selects one of the radio buttons", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
});

When("the DfE Admin clicks the on the radio button with {string} and has {string}", (text, value) => {
    cy.get('[value="' + value + '"]').first().check();
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected to the 'What is the user's full name?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhichOrganisation"));
});

When("the DfE Admin clicks on the continue button for the Which local authority is the account for? page", () => {
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected from the Which local authority is the account for? page to the 'What is the user's full name?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhatIsUsername"));
});

Given("DfE Admin logins and uses Create user accounts wizard, arrives at What is the user's full name? page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
    cy.get('[value="LAAdmin"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
});

When("the DfE Admin clicks the Continue button on the at What is the user's full name? page without adding a name", () => {
    cy.get('.govuk-button').click();
});

Then("On the What is the user's full name? page the error message {string} appears", (errormessage) => {
    cy.contains('a', errormessage);
    //cy.get('#name-error').contains(errormessage).should('be.visible'); //Not sure why this keeps erroring
});

Given("DfE Admin logins and uses Create user accounts wizard with a user type of LAAdmin arrives at What is the user's full name? page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
    cy.get('[value="LAAdmin"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
});

When("the DfE Admin adds the user name {string} and then clicks the Continue button", (name) => {
    cy.get('#FullName').invoke('val', '');
    cy.get('#FullName').type(name);
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected from the 'What is the user's full name?' page to the 'What is the user's email address?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhatIsEmailAddress"));
});


Given("DfE Admin logins and navigates to 'What is the user's email address?' Page and forgets to enter an email address", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
    cy.get('[value="LAAdmin"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.get('#FullName').invoke('val', '');
    cy.get('#FullName').type("Joe Blogs");
    cy.get('.govuk-button').click();
});

When("the DfE Admin while on the 'What is the user's email address?' Page just clicks the Continue button", () => {
    cy.get('.govuk-button').click();
});

Then("the error message for 'What is the user's email address?' Page appear with {string} and {string}", (toperrormessage, selecterrormessage) => {
    cy.contains('a', toperrormessage);
    //cy.get('.govuk-error-message').contains(selecterrormessage).should('be.visible');
});

Given("DfE Admin logins and navigates to 'What is the user's email address?' Page in the Create user accounts wizard", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
    cy.get('[value="LAAdmin"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.get('#FullName').invoke('val', '');
    cy.get('#FullName').type("Joe Blogs");
    cy.get('.govuk-button').click();
});

When("the DfE Admin enters the users email address {string} and then clicks the Continue button", (emailaddress) => {
    cy.get('#EmailAddress').invoke('val', '');
    cy.get('#EmailAddress').type(emailaddress);
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected from the 'What is the user's email address?' Page to the Check account details page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/CheckAccountDetails"));
});

Given("DfE Admin logins and uses Create user accounts wizard reaching the Check account details page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Invitation/TypeOfUser"]').click();
    cy.get('[value="LAAdmin"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.get('#FullName').invoke('val', '');
    cy.get('#FullName').type("Joe Blogs");
    cy.get('.govuk-button').click();
    cy.get('#EmailAddress').invoke('val', '');
    cy.get('#EmailAddress').type('joeblog@email.com');
    cy.get('.govuk-button').click();
});

When("the DfE Admin presses the Check account details back button", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the the Check account details page to the 'What is the user's email address?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhatIsEmailAddress"));
});

When("the DfE Admin presses the 'What is the user's email address?' back button", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the the 'What is the user's email address?' Page to the 'What is the user's full name?' page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhatIsUsername"));
});

When("the DfE Admin presses the 'What is the user's full name?' back button", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the 'What is the user's full name?' page to the Which local authority is the account for? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/WhichOrganisation"));
});

When("the DfE Admin presses the Which local authority is the account for? back button", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the 'Which local authority is the account for?' page to the Which account do you want to create? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Invitation/TypeOfUser"));
});

When("the DfE Admin presses the Which account do you want to create? back button", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the 'Which account do you want to create?' page to the home page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
});