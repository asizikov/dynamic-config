FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/clients/DatabasePrimer.Service/*.csproj", "src/clients/DatabasePrimer.Service/"]
COPY ["src/DynamicConfig.Database/*.csproj", "src/DynamicConfig.Database/"]
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