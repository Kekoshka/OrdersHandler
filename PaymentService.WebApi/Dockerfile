FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PaymentService.WebApi/PaymentService.WebApi.csproj", "PaymentService.WebApi/"]
COPY ["PaymentService.DataAccess.Postgres/PaymentService.DataAccess.Postgres.csproj", "PaymentService.DataAccess.Postgres/"]

COPY ["packages/", "/src/packages/"]

RUN dotnet restore "PaymentService.WebApi/PaymentService.WebApi.csproj" \
    --source /src/packages \
    --source https://api.nuget.org/v3/index.json

COPY PaymentService.WebApi/ PaymentService.WebApi/
COPY PaymentService.DataAccess.Postgres/ PaymentService.DataAccess.Postgres/

WORKDIR "/src/PaymentService.WebApi"
RUN dotnet build "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.WebApi.dll"]