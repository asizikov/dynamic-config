FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DynamicConfig.Storage.Api/DynamicConfig.Storage.Api.csproj", "src/DynamicConfig.Storage.Api/"]
RUN dotnet restore "src/DynamicConfig.Storage.Api/DynamicConfig.Storage.Api.csproj"
COPY . .
WORKDIR "/src/src/DynamicConfig.Storage.Api"
RUN dotnet build "DynamicConfig.Storage.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicConfig.Storage.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicConfig.Storage.Api.dll"]