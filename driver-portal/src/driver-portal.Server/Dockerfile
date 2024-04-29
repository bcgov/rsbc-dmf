#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG BUILD_ID
ARG BUILD_VERSION
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG BUILD_ID
ARG BUILD_VERSION
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_ID
ARG BUILD_VERSION
COPY . .
RUN dotnet restore ./driver-portal/src/
RUN dotnet test ./driver-portal/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID
ARG BUILD_VERSION
RUN dotnet publish ./driver-portal/src/ /p:Version="$BUILD_VERSION" /p:InformationalVersion="$BUILD_ID" -c Release -o /app/publish --no-restore 

FROM base AS final
ARG BUILD_ID
ARG BUILD_VERSION
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "driver-portal.dll"]