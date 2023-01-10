import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on user services home page", () => {
    cy.visit(`/`);
});

Then("the heading should say {string}", (heading) => {
    cy.get("h1").should("contain.text", heading);
});

Given("a user has arrived at the user services home page", () => {
    cy.visit(`/`);
});

Then("the user clicks on start now button should redirect to login page", () => {
    cy.get('[data-startnow="start-now"]').click();
    cy.location('pathname').should('match', new RegExp("/Gds/Account/Login"));
});

Given("a user has arrived at the login page", () => {
    cy.visit(`/Gds/Account/Login`);
});

Then("the login page heading should say {string} and the subheading should say {string}", (heading,subheading) => {
    cy.get("h1").should("contain.text", heading);
    cy.get("h2").should("contain.text", subheading);
});

Given("a user is on the Login page", () => {
    cy.visit(`/Gds/Account/Login`);
});

When("the user enters username as {string} and clicks login button", (username) => {
    cy.get('#Input_Email').type(username);
    cy.get('#login-submit').click();
    cy.wait(50);
});

Then("the error message {string} and {string} is displayed", (usernameerror, passworderror) => {
    cy.get('#Input_Email-error').should("contain.text", usernameerror);
    //cy.get('#Input_Password-error').should("contain.text", passworderror);
});

Given("a user is on the Login page and is entering either the wrong username or password", () => {
    cy.visit(`/Gds/Account/Login`);
});

When("the user enters username as {string} and password as {string} and clicks login button", (username,password) => {
    cy.get('#Input_Email').type(username);
    cy.get('#Input_Password').type(password);
    cy.get('#login-submit').click();  
});

Then("the error message {string} is displayed", (errormessage) => {
    cy.get('ul')
        .find('li:last-child')
        .contains(errormessage)
});

Given("a DfE Admin user is on the Login page and is entering the correct username and password", () => {
    cy.visit(`/Gds/Account/Login`);
});

When("the DfE Admin enters username as {string} and password as {string} and clicks login button", (username, password) => {
    cy.get('#Input_Email').type(username);
    cy.get('#Input_Password').type(password);
    cy.get('#login-submit').click();
});

Then("the user clicks on the login button they should be redirect to the home page", () => {
    cy.location('pathname').should('match', new RegExp("/Gds/Manage/Homepage"));
});

