FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["global.json", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["src/RadiusSearch.Api/RadiusSearch.Api.csproj", "src/RadiusSearch.Api/"]
COPY ["src/RadiusSearch.Application/RadiusSearch.Application.csproj", "src/RadiusSearch.Application/"]
COPY ["src/RadiusSearch.Domain/RadiusSearch.Domain.csproj", "src/RadiusSearch.Domain/"]
COPY ["src/RadiusSearch.Infrastructure/RadiusSearch.Infrastructure.csproj", "src/RadiusSearch.Infrastructure/"]

RUN dotnet restore "src/RadiusSearch.Api/RadiusSearch.Api.csproj"

COPY . .

RUN dotnet publish "src/RadiusSearch.Api/RadiusSearch.Api.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN adduser --disabled-password --gecos "" appuser \
    && mkdir -p /app/logs \
    && chown -R appuser:appuser /app

USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RadiusSearch.Api.dll"]
