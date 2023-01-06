import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on the services page", () => {
    cy.visit(`/`);
});

Then("the user clicks on start now button should redirect to login page", () => {
    cy.get('[data-testid="start-now"]').click();
    cy.location('pathname').should('match', new RegExp("/Gds/Account/Login"));
});