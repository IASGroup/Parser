FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/TaskManager/TaskManager.csproj", "Services/TaskManager/"]
RUN dotnet restore "Services/TaskManager/TaskManager.csproj"
COPY . .
WORKDIR "/src/Services/TaskManager"
RUN dotnet build "TaskManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.dll"]
