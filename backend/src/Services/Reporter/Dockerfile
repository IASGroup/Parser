FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Reporter/Reporter.csproj", "Services/Reporter/"]
RUN dotnet restore "Services/Reporter/Reporter.csproj"
COPY . .
WORKDIR "/src/Services/Reporter"
RUN dotnet build "Reporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reporter.dll"]
