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
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected to the 'What is the local authority's name?' Page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/OrganisationName"));
});

Given("DfE Admin logins and uses Add New Organisation wizard, arrives at What is the local authority's name? page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
});

When("the DfE Admin clicks the Continue button on the at What is the local authority's name? page without adding a name", () => {
    cy.get('.govuk-button').click();
});

Then("On the What is the local authority's name? page the error message {string} and {string} appear", (toperrormessage, nameerrormessage) => {
    cy.contains('a', toperrormessage);
    cy.get('.govuk-error-message').contains(nameerrormessage).should('be.visible');
});

Given("DfE Admin logins and uses Add New Organisation wizard, progresses to the What is the local authority's name? page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
});

When("at What is the local authority's name? page, the DfE Admin types a name {string} and clicks the Continue button", (name) => {
    cy.get('#Name').type(name);
    cy.get('.govuk-button').click();
});

Then("the DfE Admin is redirected to the Check details Page having a heading of {string}", (heading) => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/CheckOrganisationDetails"));
    cy.get(".govuk-fieldset__heading").should("contain.text", heading);
});

Given("DfE Admin logins and uses Add New Organisation wizard through to the check details page", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type("Test LA");
    cy.get('.govuk-button').click();
});

When("the user press the back button on the Check Details page", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected to the What is the local authority's name? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/OrganisationName"));
});

When("the user press the next back button on the What is the local authority's name? page", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the What is the local authority's name? page to the Which local authority is the organisation in? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/WhichLAOrAdminDistrict"));
});

When("the user press the next back button on the Which local authority is the organisation in? page", () => {
    cy.get('.govuk-back-link').contains('Back').click();
});

Then("the DfE Admin is redirected from the Which local authority is the organisation in? page to the Type of Organisation page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/TypeOfOrganisation"));
});

Given("DfE Admin logins and uses Add New Organisation wizard and wants to change the 'What you added' entry", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type("Test LA");
    cy.get('.govuk-button').click();
});

When("the user press the change link for What you added", () => {
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
});

Then("the DfE Admin is redirected from the Check Details page to the Type of Organisation page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/TypeOfOrganisation"));
});

Given("DfE Admin logins and uses Add New Organisation wizard and wants to change the 'Local Authority' entry", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type("Test LA");
    cy.get('.govuk-button').click();
});

When("the user press the change link for Local Authority", () => {
    cy.get('a[href*="/Gds/OrganisationWizard/WhichLAOrAdminDistrict"]').click();
});

Then("the DfE Admin is redirected from the Check Details page to the Which local authority is the organisation in? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/WhichLAOrAdminDistrict"));
});

Given("DfE Admin logins and uses Add New Organisation wizard and wants to change the 'Name' entry", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type("Test LA");
    cy.get('.govuk-button').click();
});

When("the user press the change link for Name of the organisation", () => {
    cy.get('a[href*="/Gds/OrganisationWizard/OrganisationName"]').first().click();
});

Then("the DfE Admin is redirected from the Check Details page to the What is the local authority's name? page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/OrganisationName"));
});

Given("DfE Admin logins and uses Add New Organisation wizard and enters all the data", () => {
    cy.userservicelogin("DfEAdmin@email.com", "Pass123$");
    cy.get('a[href*="/Gds/OrganisationWizard/TypeOfOrganisation"]').click();
    cy.get('[value="LA"]').first().check();
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('.govuk-button').click();
    cy.wait(500);
    cy.get('#Name').invoke('val', '');
    cy.get('#Name').type("Test LA");
    cy.get('.govuk-button').click();
});

When("the user press the Confirm details button", () => {
    cy.get('.govuk-button').contains('Confirm details').click();
});

Then("the DfE Admin is redirected from the Check Details page to the Confirmation page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/OrganisationWizard/Confirmation"));
});