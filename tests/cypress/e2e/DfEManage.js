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

Given("a DfE Admin is on the View Organisations page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin selects the Filter", () => {
    
});

Then("I should select {string} with {string}", (value,text) => {

    cy.get('#' + value).check();
    cy.get('button[type=submit]').click();
    cy.get("tr td:nth-child(2)")       //Gets the 2nd child in td column
        .eq(1)                        //Yields second matching css element 
        .contains(text)
        .should('be.visible');

    
});


Given("a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin clicks all the filters follwed by apply filter button", () => {
    cy.get('#LA').check();
    cy.get('#VCFS').check();
    cy.get('#FamilyHub').check();
    cy.get('#Company').check();
    cy.get('button[type=submit]').click();
    cy.get(".govuk-table").find("tr").find("td").contains("Family Hub");
    cy.get(".govuk-table").find("tr").find("td").contains("Local Authority");
    cy.get('[type="checkbox"]').should('be.checked');
});

Then("presses clear filters link and the apply filter button, all results are shown", () => {
    cy.get('a[href*="/Gds/Manage/ViewOrganisations?handler=ClearFilter"]').click();
    cy.get('button[type=submit]').click();
    cy.get(".govuk-table").find("tr").find("td").contains("Family Hub");
    cy.get(".govuk-table").find("tr").find("td").contains("Local Authority");  
    cy.get('[type="checkbox"]').should('not.be.checked');
});