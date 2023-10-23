FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Migrations/Migrations.csproj", "Services/Migrations/"]
RUN dotnet restore "Services/Migrations/Migrations.csproj"
COPY . .
WORKDIR "/src/Services/Migrations"
RUN dotnet build "Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Migrations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY "Services/Migrations/config.json" .
COPY "Services/Migrations/config-docker.json" .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migrations.dll"]
