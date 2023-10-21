FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Hubs/Hubs.csproj", "Services/Hubs/"]
RUN dotnet restore "Services/Hubs/Hubs.csproj"
COPY . .
WORKDIR "/src/Services/Hubs"
RUN dotnet build "Hubs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hubs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hubs.dll"]
