FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DynamicConfig.Management.Web/*.csproj", "src/DynamicConfig.Management.Web/"]
COPY ["src/DynamicConfig.Database/*.csproj", "src/DynamicConfig.Database/"]
COPY ["src/DynamicConfig.Storage.DatabaseModel/*.csproj", "src/DynamicConfig.Storage.DatabaseModel/"]
COPY ["src/DynamicConfig.Storage.Model/*.csproj", "src/DynamicConfig.Storage.Model/"]
RUN dotnet restore "src/DynamicConfig.Management.Web/DynamicConfig.Management.Web.csproj"
COPY . .
WORKDIR "/src/src/DynamicConfig.Management.Web"
RUN dotnet build "DynamicConfig.Management.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicConfig.Management.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicConfig.Management.Web.dll"]