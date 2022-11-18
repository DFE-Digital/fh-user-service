# fh-user-service

## Requirements

* DotNet Core 6.0 and any supported IDE for DEV running.

## About

This is a Razor Pages application for managing users and organisations that access the Family Hubs service directory, and the Family Hubs Referral Service. 

There are also an API used for authentication in order to access Access Tokens and Refresh Tokens.

## Local running

To run this application you will also need to clone and run the service directory api: https://github.com/DFE-Digital/fh-service-directory-api

In the appsetting.json file set:

* "ServiceDirectoryUrl": "https://localhost:7022/",
* "UseDbType": "UseInMemoryDatabase"

The UseDbType options are: 

* UseInMemoryDatabase
* SqlLite
* SqlServerDatabase
* Postgres

The startup project is: FamilyHub.IdentityServerHost

## API Login

Send a post command to: https://localhost:7108/api/Authenticate/login

With the body:

{
  "username": "your username",
  "password": "your password"
}

## Migrations Commands

First of all add this package if it does not already exist.
dotnet add package Microsoft.EntityFrameworkCore.Design

To add migrations from scratch:

dotnet ef migrations add CreateIdentitySchema -c ApplicationDbContext --output-dir Persistence\Data\CreateIdentitySchema

After you have added any migrations update the database with:

dotnet ef database update -c ApplicationDbContext


