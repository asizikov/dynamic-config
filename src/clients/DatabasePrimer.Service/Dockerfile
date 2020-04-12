FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/clients/DatabasePrimer.Service/*.csproj", "src/clients/DatabasePrimer.Service/"]
COPY ["src/DynamicConfig.Storage.Model/*.csproj", "src/DynamicConfig.Storage.Model/"]
COPY ["src/DynamicConfig.Storage.DatabaseModel/*.csproj", "src/DynamicConfig.Storage.DatabaseModel/"]
RUN dotnet restore "src/clients/DatabasePrimer.Service/DatabasePrimer.Service.csproj"
COPY . .
WORKDIR "/src/src/clients/DatabasePrimer.Service/"
RUN dotnet build "DatabasePrimer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabasePrimer.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabasePrimer.Service.dll"]