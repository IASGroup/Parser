start powershell -noexit -command "$Host.UI.RawUI.WindowTitle = 'TaskManager';dotnet run --project './TaskManager/TaskManager.csproj'"
start powershell -noexit -command "$Host.UI.RawUI.WindowTitle = 'Collector';dotnet run --project './Collector/Collector.csproj'"
start powershell -noexit -command "$Host.UI.RawUI.WindowTitle = 'Hubs';dotnet run --project './Hubs/Hubs.csproj'"
start powershell -noexit -command "$Host.UI.RawUI.WindowTitle = 'Gateway';dotnet run --project './Gateway/Gateway.csproj'"