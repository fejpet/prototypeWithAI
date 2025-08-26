# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Másoljuk a projekteket és visszaállítjuk a függőségeket
COPY *.sln .
COPY src/OrganizationService/*.csproj ./src/OrganizationService/
COPY src/OrganizationService.Tests/*.csproj ./src/OrganizationService.Tests/
RUN dotnet restore

# Másoljuk a teljes forráskódot
COPY . .
WORKDIR /source/src/OrganizationService
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrganizationService.dll"]
