FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Unlocker/Unlocker.csproj", "Services/Unlocker/"]
RUN dotnet restore "Services/Unlocker/Unlocker.csproj"
COPY . .
WORKDIR "/src/Services/Unlocker"
RUN dotnet build "Unlocker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Unlocker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unlocker.dll"]
