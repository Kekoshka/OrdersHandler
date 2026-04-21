FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OrderService.WebApi/OrderService.WebApi.csproj", "OrderService.WebApi/"]
COPY ["OrderService.DataAccess.Postgres/OrderService.DataAccess.Postgres.csproj", "OrderService.DataAccess.Postgres/"]

COPY ["packages/", "/src/packages/"]

RUN dotnet restore "OrderService.WebApi/OrderService.WebApi.csproj" \
    --source /src/packages \
    --source https://api.nuget.org/v3/index.json

COPY OrderService.WebApi/ OrderService.WebApi/
COPY OrderService.DataAccess.Postgres/ OrderService.DataAccess.Postgres/

WORKDIR "/src/OrderService.WebApi"
RUN dotnet build "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]