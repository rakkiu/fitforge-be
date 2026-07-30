FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/FitForge.Core/FitForge.Core.csproj src/FitForge.Core/
COPY src/FitForge.Shared/FitForge.Shared.csproj src/FitForge.Shared/
COPY src/FitForge.Infrastructure/FitForge.Infrastructure.csproj src/FitForge.Infrastructure/
COPY src/FitForge.Api/FitForge.Api.csproj src/FitForge.Api/
RUN dotnet restore src/FitForge.Api/FitForge.Api.csproj

# Copy source and build
COPY src/ src/
RUN dotnet publish src/FitForge.Api/FitForge.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FitForge.Api.dll"]
