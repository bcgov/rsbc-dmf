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
RUN dotnet restore ./system-status/src/
RUN dotnet test ./system-status/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID
ARG BUILD_VERSION
RUN echo "$BUILD_VERSION"
RUN dotnet publish ./system-status/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release -o /app/publish --no-restore 

FROM base AS final
ARG BUILD_ID
ARG BUILD_VERSION
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "system-status.dll"]