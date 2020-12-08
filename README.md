# Etimo ID

Etimo ID is an [OAuth2](https://tools.ietf.org/html/rfc6749#section-5.2) compliant api that aims to be a simple-to-use implementation of OAuth2.

At a later stage, [OpenID Connect](https://openid.net/specs/openid-connect-core-1_0.html) will also be implemented.

## Prerequisites

You need [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0) and [Docker Desktop](https://www.docker.com/products/docker-desktop) (or just Docker if you're running Linux).

You also need to install `dotnet-ef`:

```
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-user-secrets
```

## Secrets

This project uses `dotnet user-secrets`, which means you need to setup the following secrets in the `Etimo.Id.Api` project:

```
JwtSettings:Secret
ConnectionStrings:EtimoId
```

* The STRONGSECRET should be at least 32 characters long.
* The CONNECTIONSTRING should be the connection string to your database.

To setup some default values you can use while developing, use the `setup-secrets.ps1` script.

You should **REALLY** change the Secret value in your production environment.

## Setting up database

Start the database server by typing `docker-compose up -d`

You can access the database GUI from https://localhost:8011

## Building / running

Run etimo-id by typing `dotnet run --project Etimo.Id.Api`

Or by using the `./scripts/run.ps1` script.

### Creating your first users

When starting etimo-id for the first time, a pre-seeded user and application is added:

The user has username `admin` with password `etimo`, while the client has the client_id `11111111-1111-1111-1111-111111111111` and password `etimo`.

The system seeds a default application called `etimo-default` with an `admin` role that lets you setup the system.

You should delete these accounts when you have added your own admin accounts/applications.

## Commit style

This project uses [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) in this project.

### Valid commit types

Type | Description
--- | ---
feat | New feature
fix | Bug fix
docs | Changes to documentation
style | Formatting, etc; no code change
refactor | Code refactoring
test | Write and refactor tests; no code change
chore | Adding/updating scripts, configs, etc; no code change
