import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a DfE Admin logs in and goes to the Manage user accounts Page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
});

When("the DfE Admin clicks on the 'Manage user accounts' link", () => {
    cy.get('a[href*="/Gds/Manage/ViewUsers"]').click();
});

Then("the DfE Admin is redirected to Manage user accounts Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/ViewUsers"));
});

Given("DfE Admin logins and navigates to Manage user accounts Page  and then wants to go back", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Manage/ViewUsers"]').click();
});

When("the DfE Admin clicks on the Manage user accounts back link", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected back to the Home page Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
});
