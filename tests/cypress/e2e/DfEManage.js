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


Given("a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations and then wants to go back", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin clicks on the ViewOrganisations back link", (username, password) => {
    cy.get('a[href*="/Gds/Manage/Homepage"]').click();
});

Then("the DfE Admin is redirected back to the View Organisations Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
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

Given("a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page to view details", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin clicks on the Tower Hamlets View Link", () => {
    cy.get('a[href*="/Gds/Manage/ViewOrganisationDetail?id=88e0bffd-ed0b-48ea-9a70-5f6ef729fc21"]').click();
});

Then("the DfE Admin is redirect to the details page with the heading as {string}", (heading) => {
    cy.get("h1").should("contain.text", heading);
});

Given("a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page to view item details", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

When("the DfE Admin clicks on Back to manage local authorities and voluntary community organisations", () => {
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
});

Then("the DfE Admin is redirect back to the Manage local authorities and voluntary community organisations page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/ViewOrganisations"));
});




Given("a DfE Admin logs in and navigates to Manage local authorities and voluntary community organisations page then alters organisation name", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$")
    cy.get('a[href*="/Gds/Manage/ViewOrganisations"]').click();
    cy.get('a[href*="/Gds/Manage/ViewOrganisationDetail?id=88e0bffd-ed0b-48ea-9a70-5f6ef729fc21"]').click();
});

When("the DfEAdmin should modify the name to {string} and press save details", (value) => {
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type(value);
    cy.get('.govuk-button').click();
});

Then("Local Authority name should be changed to {string}", (value) => {

    cy.get(".govuk-summary-list__value").contains(value);

});

Given("a DfE Admin logs in and goes to Manage local authorities and voluntary community organisations and then to the deatils page then wants to go back", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.visit(`/Gds/Manage/ViewOrganisationDetail?id=88e0bffd-ed0b-48ea-9a70-5f6ef729fc21`);
});

When("the DfE Admin clicks on the View Organisations Details page back link", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected back to the Manage local authorities and voluntary community organisations Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/ViewOrganisations"));
});

