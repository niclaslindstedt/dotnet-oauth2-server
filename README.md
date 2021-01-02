[![Build](https://github.com/Etimo/etimo-id/workflows/Build/badge.svg?branch=develop)](https://github.com/Etimo/etimo-id/actions?query=workflow%3ABuild) [![Tests](https://github.com/Etimo/etimo-id/workflows/Tests/badge.svg?branch=develop)](https://github.com/Etimo/etimo-id/actions?query=workflow%3ATests) [![codecov](https://codecov.io/gh/Etimo/etimo-id/branch/develop/graph/badge.svg?token=3TJPDMKNRT)](https://codecov.io/gh/Etimo/etimo-id) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/Etimo/etimo-id/blob/master/LICENSE)

# Etimo ID

Etimo ID is a basic implementation of [OAuth2](https://tools.ietf.org/html/rfc6749#section-5.2), without all the bloat.

At a later stage, [OpenID Connect](https://openid.net/specs/openid-connect-core-1_0.html) will also be implemented.

## Prerequisites

You need both [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) and [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0).

You also need [Docker Desktop](https://www.docker.com/products/docker-desktop) if you plan on running this in Docker.

To install the required dotnet tools, type:

```
dotnet tool restore
```

## Secrets

This project uses `dotnet user-secrets`, which means you need to setup the following secrets in the `Etimo.Id.Api` project:

```
dotnet user-secrets set JwtSettings:Secret STRONGSECRET
dotnet user-secrets set ConnectionStrings:EtimoId CONNECTIONSTRING
```

* The STRONGSECRET should be at least 32 characters long.
* The CONNECTIONSTRING should be the connection string to your database.

To setup some default values you can use while developing, use the `setup-secrets.csx` script.

You should **REALLY** change the Secret value in your production environment.

## Setting up database

Start the database server by typing `docker-compose up -d`

You can access the database GUI from https://localhost:8011

## Building / running

Run etimo-id by typing `dotnet run --project Etimo.Id.Api`

Or by using the `run.csx` script.

The project is served from https://localhost:5011

## Debugging

In VSCode, simply press `F5` to start a debugging session (it should use the `.NET Core Run` debugger).

## Docker

You can use the `dev.csx` script to build and run etimo-id in a Docker container.

It has debugging enabled, so you can simply attach to the process using the `.NET Core Attach Docker` debugger in VSCode.

This will open a list of processes. Select `Etimo.Id.Api` from the list of processes.

Now you can set breakpoints in the code and debug as usual. It is a bit slower than non-container debugging.

The project is served from https://localhost:5011

### Creating your first users

When starting etimo-id for the first time, a default user and application is added:

```
username: admin
password: etimo
client_id: 11111111-1111-1111-1111-111111111111
client_secret: etimo
```

Use this user to setup the system. When you are done setting up, you should delete this user.

If you want to remove the database and start over, use the `delete-database.csx` script.

## Formatting

View the `.editorconfig` file for a full list of formatting rules.

To install a pre-commit hook that lints the code before committing, use the `install-hooks.csx` script.

## Commit style

This project uses [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) for its commit messages.

The project uses `husky` + `commitlint` to validate commit messages. Type `npm install` to install these.

### Valid commit types

Type | Description
--- | ---
chore | Adding/updating scripts, configs, etc; no code change
ci | Changes to continuous integration
docs | Changes to documentation
feat | New feature
fix | Bug fix
refactor | Code refactoring
repo | Updates to e.g. git hooks
scripts | Updates to scripts in repository
test | Write and refactor tests; no code change
wip | Work in progress -- use in feature branches where you squash merge
