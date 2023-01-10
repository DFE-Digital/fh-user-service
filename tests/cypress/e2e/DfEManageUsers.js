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


Given("DfEAdmin on the Manage user accounts Page and selects each filter in turn", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Manage/ViewUsers"]').click();
});


When("the DfE Admin selects the Filter", () => {

});

Then("I should select {string} with {string}", (value, text) => {

    cy.get('#' + value).check();
    cy.get('button[type=submit]').click();
    cy.get('.govuk-table').contains('td', text).should('be.visible');
});


Given("DfE Admin logins and navigates to Manage user accounts Page and sets all the checkbox filters", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Manage/ViewUsers"]').click();
});


When("the DfE Admin clicks all the filters follwed by apply filter button", () => {
    cy.get('[type="checkbox"]').check();
    cy.get('button[type=submit]').click();
});

Then("presses the clear filters link, all the results are then shown without filters", () => {
    cy.get('a[href*="/Gds/Manage/ViewUsers?handler=ClearFilters"]').click();
    cy.get('[type="checkbox"]').should('not.be.checked');
});

Given("a DfE Admin logs in and navigates to Manage Manage user accounts page to view a users details", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewUsers"]').click();
});

When("the DfE Admin clicks on the {string} View Link", (user) => {
    cy.get('a[href*="/Gds/Manage/ViewUser?emailAddress=' + user + '"]').click();
});

Then("the DfE Admin is redirect to the details page with the heading as {string}", (heading) => {
    cy.get("h1").should("contain.text", heading);
});


Given("a DfE Admin logs in and goes to Manage user accounts page and then to the details page then wants to go back to the previous page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.visit(`/Gds/Manage/ViewUser?emailAddress=BtlLAAdmin@email.com`);
});

When("the DfE Admin in the view user page clicks on the {string} link", (linktext) => {
    cy.get('a').contains(linktext).click();
});

Then("the DfE Admin is redirected back to the Manage user accounts page Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/ViewUsers"));
});

Given("a DfE Admin logs in and goes to Manage user accounts page and then to the details page then presses delete user link", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.visit(`/Gds/Manage/ViewUser?emailAddress=BtlLAAdmin@email.com`);
});

When("the DfE Admin in the view user page clicks on the delete user link", () => {
    cy.get('a').contains("Delete Account").click();
});

Then("the DfE Admin is redirected to delete user page", () => {
    cy.location('pathname').should('match', new RegExp("/Identity/Account/DeleteUser"));
});
