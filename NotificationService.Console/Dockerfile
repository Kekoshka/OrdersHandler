# NotificationService.Console/Dockerfile
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["NotificationService.Console/NotificationService.Console.csproj", "NotificationService.Console/"]
RUN dotnet restore "NotificationService.Console/NotificationService.Console.csproj"

COPY NotificationService.Console/ NotificationService.Console/
WORKDIR "/src/NotificationService.Console"
RUN dotnet build "NotificationService.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.Console.dll"]