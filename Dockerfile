FROM mcr.microsoft.com/dotnet/sdk:5.0 as base
WORKDIR /app
COPY etimo-id.sln .
COPY src/Etimo.Id.Abstractions/*.csproj ./src/Etimo.Id.Abstractions/
COPY src/Etimo.Id.Api/*.csproj ./src/Etimo.Id.Api/
COPY src/Etimo.Id.Data/*.csproj ./src/Etimo.Id.Data/
COPY src/Etimo.Id.Entities/*.csproj ./src/Etimo.Id.Entities/
COPY src/Etimo.Id.Entities.Abstractions/*.csproj ./src/Etimo.Id.Entities.Abstractions/
COPY src/Etimo.Id.Service/*.csproj ./src/Etimo.Id.Service/
RUN dotnet restore
COPY . .

FROM base as build
RUN dotnet build \
    --configuration Release \
    --no-restore \
    --output ./out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as prod
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 443
ENTRYPOINT dotnet Etimo.Id.Api.dll