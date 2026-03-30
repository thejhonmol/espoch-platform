FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/src/ESPOCH.API/ESPOCH.API.csproj ESPOCH.API/
COPY backend/src/ESPOCH.Core/ESPOCH.Core.csproj ESPOCH.Core/
COPY backend/src/ESPOCH.Infrastructure/ESPOCH.Infrastructure.csproj ESPOCH.Infrastructure/
RUN dotnet restore ESPOCH.API/ESPOCH.API.csproj

COPY backend/src/ESPOCH.API/ ESPOCH.API/
COPY backend/src/ESPOCH.Core/ ESPOCH.Core/
COPY backend/src/ESPOCH.Infrastructure/ ESPOCH.Infrastructure/

WORKDIR /src/ESPOCH.API
RUN dotnet publish ESPOCH.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ESPOCH.API.dll"]
