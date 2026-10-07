FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HMS.Domain/HMS.Domain.csproj HMS.Domain/
COPY HMS.Application/HMS.Application.csproj HMS.Application/
COPY HMS.Infrastructure/HMS.Infrastructure.csproj HMS.Infrastructure/
COPY HMS.API/HMS.API.csproj HMS.API/
RUN dotnet restore HMS.API/HMS.API.csproj

COPY . .
RUN dotnet publish HMS.API/HMS.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "HMS.API.dll"]