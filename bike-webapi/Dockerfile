FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["bike-webapi.csproj", "./"]
RUN dotnet restore "./bike-webapi.csproj"
COPY . .
RUN dotnet build "bike-webapi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "bike-webapi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY "*.csv" ./
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "bike-webapi.dll"]
