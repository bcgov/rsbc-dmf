ARG BUILD_ID
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG BUILD_ID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY . .
RUN dotnet restore "./partner-portal/src/API/"
# RUN dotnet test "./partner-portal/src/API/RSBC.DMF.MedicalPortal.API/" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID

RUN dotnet publish /p:InformationalVersion="$BUILD_ID" "./partner-portal/src/API/partner-portal.Server.csproj" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "partner-portal.Server.dll"]