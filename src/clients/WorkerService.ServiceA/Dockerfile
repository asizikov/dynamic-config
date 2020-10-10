FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/clients/WorkerService.ServiceA", "clients/WorkerService.ServiceA/"]
COPY ["src/DynamicConfig.Configuration", "DynamicConfig.Configuration/"]
COPY ["src/DynamicConfig.Storage.Model", "DynamicConfig.Storage.Model/"]
RUN dotnet restore "clients/WorkerService.ServiceA/WorkerService.ServiceA.csproj"

WORKDIR "/src/clients/WorkerService.ServiceA/"
RUN dotnet build "WorkerService.ServiceA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerService.ServiceA.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerService.ServiceA.dll"]