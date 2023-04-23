ARG BUILD_ID
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG BUILD_ID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . .
RUN dotnet restore "./medical-portal/src/PidpAdpater.API/"
RUN dotnet test "./medical-portal/src/PidpAdpater.API/" /p:InformationalVersion="$BUILD_ID" -c Release

FROM build AS publish
ARG BUILD_ID

RUN dotnet publish /p:InformationalVersion="$BUILD_ID" "./medical-portal/src/PidpAdpater.API/PidpAdapter.API.csproj" -c Release -o /app/publish --no-restore 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

CMD ["./PidpAdapter.API"]