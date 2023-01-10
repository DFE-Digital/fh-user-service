import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";



Given("a DfE Admin logs in and goes to the Add a local authority or voluntary community organisation wizard", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
});

When("the DfE Admin clicks on the 'Add a local authority or voluntary community organisation' link", () => {
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
});

Then("the DfE Admin is redirected to the Type Of Organisation Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/TypeOfOrganisation"));
});

Given("DfE Admin logins and navigates to Add New Organisation Page  and then wants to go back", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
});

When("the DfE Admin clicks on the back link", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected back to the Home page Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
});

Given("DfE Admin logins and navigates to Add New Organisation Page and and forgets to select one of the radio buttons", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
});

When("the DfE Admin clicks the Continue button", () => {
    cy.get('.govuk-button').click();
});

Then("the error message {string} and {string} appear", (toperrormessage, selecterrormessage) => {
    cy.contains('a', toperrormessage);
    cy.get('.govuk-error-message').contains(selecterrormessage).should('be.visible');
});

Given("DfE Admin logins and navigates to Add New Organisation Page and select one of the radio buttons", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
});

When("the DfE Admin clicks the on the radio button with {string} and has {string}", (text, value) => {
    cy.get('[value="' + value + '"]').first().check();
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected to the 'Which local authority is the organisation in?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/WhichLAOrAdminDistrict"));
});

Given("DfE Admin logins and navigates to Which local authority is the organisation in? Page makes selection and continues", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();   
});

When("the DfE Admin clicks the Continue button on the Which local authority is the organisation in? Page", () => {
    //cy.get('select').select([]);
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected to the 'What is the local authority's name?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/OrganisationName"));
});
