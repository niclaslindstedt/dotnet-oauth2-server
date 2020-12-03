# Etimo ID

Etimo ID is an [OAuth2](https://tools.ietf.org/html/rfc6749#section-5.2) compliant api that aims to be a simple-to-use implementation of OAuth2.

At a later stage, [OpenID Connect](https://openid.net/specs/openid-connect-core-1_0.html) will also be implemented.

## Prerequisites

You need [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0) and [Docker Desktop](https://www.docker.com/products/docker-desktop) (or just Docker if you're running Linux).

## Create database

Start the database server by typing `docker-compose up -d`

Now seed the database with `dotnet ef database update --project Etimo.Id.Data --startup-project Etimo.Id.Api`

Or just use the `update-database.ps1` script in the `scripts/` folder.

## Building / running

Run etimo-id by typing `dotnet run --project Etimo.Id.Api`

## Commit style

We're using [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) in this project.

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
