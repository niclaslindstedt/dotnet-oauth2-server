FROM mcr.microsoft.com/dotnet/sdk:5.0 as base
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        procps \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh \
        | bash /dev/stdin -v latest -l /vsdbg
COPY etimo-id.sln Directory.Build.props ./
COPY src/Etimo.Id.Abstractions/*.csproj ./src/Etimo.Id.Abstractions/
COPY src/Etimo.Id.Api/*.csproj ./src/Etimo.Id.Api/
COPY src/Etimo.Id.Attributes/*.csproj ./src/Etimo.Id.Attributes/
COPY src/Etimo.Id.Client/*.csproj ./src/Etimo.Id.Client/
COPY src/Etimo.Id.Constants/*.csproj ./src/Etimo.Id.Constants/
COPY src/Etimo.Id.Data/*.csproj ./src/Etimo.Id.Data/
COPY src/Etimo.Id.Dtos/*.csproj ./src/Etimo.Id.Dtos/
COPY src/Etimo.Id.Entities/*.csproj ./src/Etimo.Id.Entities/
COPY src/Etimo.Id.Entities.Abstractions/*.csproj ./src/Etimo.Id.Entities.Abstractions/
COPY src/Etimo.Id.Exceptions/*.csproj ./src/Etimo.Id.Exceptions/
COPY src/Etimo.Id.Security/*.csproj ./src/Etimo.Id.Security/
COPY src/Etimo.Id.Service/*.csproj ./src/Etimo.Id.Service/
COPY src/Etimo.Id.Tests/*.csproj ./src/Etimo.Id.Tests/
RUN dotnet restore
COPY . .

FROM base as dev
VOLUME /app/src
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
CMD dotnet watch --project ./src/Etimo.Id.Api run --no-restore

FROM base as build
RUN dotnet build \
    --configuration Release \
    --no-restore \
    --output ./out
RUN dotnet test \
    --configuration Release \
    --no-restore \
    --no-build \
    --output ./out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as prod
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 443
ENTRYPOINT dotnet Etimo.Id.Api.dll