ARG BUILD_ID
ARG BUILD_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG BUILD_ID
ARG BUILD_VERSION

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_ID
ARG BUILD_VERSION

COPY . .
RUN echo "$BUILD_VERSION"
RUN dotnet restore ./migration-metrics/src/
RUN dotnet test ./migration-metrics/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID
ARG BUILD_VERSION
RUN echo "$BUILD_PREFIX$RUN_NUMBER"
RUN dotnet publish ./migration-metrics/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "migration-metrics.dll"]