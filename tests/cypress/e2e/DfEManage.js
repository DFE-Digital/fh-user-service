import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a DfE Admin user is on the Login page and is entering the correct username and password", () => {
    cy.visit(`/Gds/Account/Login`);
});

When("the DfE Admin enters username as {string} and password as {string} and clicks login button", (username, password) => {
    cy.userservicelogin(username, password)
});

Then("the user clicks on the login button they should be redirect to the home page and the heading should say {string} and subheading should say {string}", (heading, subheading) => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
    cy.get("h1").should("contain.text", heading);
    cy.get("h2").should("contain.text", subheading);
});

Given("a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations", () => {
    cy.visit(`/Gds/Account/Login`);
    cy.get('#Input_Email').type("DfEAdmin@email.com");
    cy.get('#Input_Password').type("Pass123$");
    cy.get('#login-submit').click();
});

When("the DfE Admin clicks on the 'Manage local authorities and voluntary community organisations' link", (username, password) => {
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

Then("the DfE Admin is redirected to View Organisations Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/ViewOrganisations"));
});

//

Given("a DfE Admin is on the View Organisations page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin selects the Filter", (datatable) => {
    datatable.forEach()
});


