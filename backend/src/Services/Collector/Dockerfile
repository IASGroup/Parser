﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Collector/Collector.csproj", "Services/Collector/"]
RUN dotnet restore "Services/Collector/Collector.csproj"
COPY . .
WORKDIR "/src/Services/Collector"
RUN dotnet build "Collector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Collector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Collector.dll"]
