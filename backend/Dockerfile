FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ESPOCH.API/ESPOCH.API.csproj", "ESPOCH.API/"]
COPY ["src/ESPOCH.Core/ESPOCH.Core.csproj", "ESPOCH.Core/"]
COPY ["src/ESPOCH.Infrastructure/ESPOCH.Infrastructure.csproj", "ESPOCH.Infrastructure/"]
RUN dotnet restore "ESPOCH.API/ESPOCH.API.csproj"
COPY . .
WORKDIR "/src/ESPOCH.API"
RUN dotnet build "ESPOCH.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ESPOCH.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ESPOCH.API.dll"]
