ARG BUILD_ID
ARG BUILD_PREFIX
ARG RUN_NUMBER

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG BUILD_ID
ARG BUILD_PREFIX
ARG RUN_NUMBER

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . .
RUN echo "$BUILD_PREFIX$RUN_NUMBER"
RUN dotnet restore ./migration-metrics/src/
RUN export BUILD_VERSION="$BUILD_PREFIX$RUN_NUMBER" && echo $BUILD_VERSION && dotnet test ./migration-metrics/src/ /p:Version="$BUILD_VERSION"  /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID
ARG BUILD_PREFIX
ARG RUN_NUMBER

RUN export BUILD_VERSION="$BUILD_PREFIX$RUN_NUMBER" && echo $BUILD_VERSION && dotnet publish /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" "./migration-metrics/src/" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "migration-metrics.dll"]